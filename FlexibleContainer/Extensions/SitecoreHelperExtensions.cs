using FlexibleContainer.Renderer;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using System.Text.RegularExpressions;
using System.Web;

namespace FlexibleContainer.Extensions
{
    public static class SitecoreHelperExtensions
    {
        private static readonly Regex TranslationRegex = new Regex(
            @"@(?<!\\)\((?<dictionaryKey>[^)]+?)(?<!\\)\)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex FieldRegex = new Regex(
            @"(?<!\\){(?<fieldName>[^}]+?)(\|editable:(?<editable>[01a-zA-Z]+?))?(?<!\\)}",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex StaticPlaceholderRegex = new Regex(
            @"^(?<!\\)\[(?<placeholderKey>.+)(?<!\\)\]$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex DynamicPlaceholderRegex = new Regex(
            @"^@(?<!\\)\[" +
            @"(?<placeholderKey>.+?)" +
            @"(\|count:(?<count>\d+?))?" +
            @"(\|maxCount:(?<maxCount>\d+?))?" +
            @"(\|seed:(?<seed>\d+?))?" +
            @"(?<!\\)\]$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public static HtmlString RenderFlexibleContainer(this SitecoreHelper helper)
        {
            Assert.ArgumentNotNull(helper, nameof(helper));

            var parameterValue = RenderingContext.Current.Rendering.Parameters["Expression"];
            var expression = string.IsNullOrWhiteSpace(parameterValue) ? "div" : parameterValue;
            var result = ExpressionRenderer.Render(expression, textFormatter);
            return new HtmlString(result);

            string textFormatter(string text)
            {
                // Field interpolation
                var fieldMatches = FieldRegex.Matches(text);
                foreach (Match fieldMatch in fieldMatches)
                {
                    var fieldName = fieldMatch.Groups["fieldName"].Value;
                    if (string.IsNullOrWhiteSpace(fieldName))
                    {
                        continue;
                    }

                    var editable = MainUtil.GetBool(fieldMatch.Groups["editable"].Value, true);
                    var field = helper.Field(fieldName, new { DisableWebEdit = !editable }).ToString();
                    text = text.Replace(fieldMatch.Value, field);
                }

                // Translation
                var translationMatches = TranslationRegex.Matches(text);
                foreach (Match translationMatch in translationMatches)
                {
                    var dictionaryKey = translationMatch.Groups["dictionaryKey"].Value;
                    text = text.Replace(translationMatch.Value, Translate.Text(dictionaryKey));
                }

                // Dynamic placeholder
                var dynamicPlaceholderMatch = DynamicPlaceholderRegex.Match(text);
                if (dynamicPlaceholderMatch.Success)
                {
                    var placeholderKey = dynamicPlaceholderMatch.Groups["placeholderKey"].Value;
                    if (!int.TryParse(dynamicPlaceholderMatch.Groups["count"].Value, out int count))
                    {
                        count = 1;
                    }
                    if (!int.TryParse(dynamicPlaceholderMatch.Groups["maxCount"].Value, out int maxCount))
                    {
                        maxCount = 0;
                    }
                    if (!int.TryParse(dynamicPlaceholderMatch.Groups["seed"].Value, out int seed))
                    {
                        seed = 0;
                    }
                    var placeholder = helper.DynamicPlaceholder(placeholderKey, count, maxCount, seed).ToString();
                    text = DynamicPlaceholderRegex.Replace(text, placeholder);
                }

                // Static placeholder
                var staticPlaceholderMatch = StaticPlaceholderRegex.Match(text);
                if (staticPlaceholderMatch.Success)
                {
                    var placeholderKey = staticPlaceholderMatch.Groups["placeholderKey"].Value;
                    var placeholder = helper.Placeholder(placeholderKey).ToString();
                    text = StaticPlaceholderRegex.Replace(text, placeholder);
                }

                return text
                    .Replace("\\[", "[")
                    .Replace("\\]", "]")
                    .Replace("\\{", "{")
                    .Replace("\\}", "}")
                    .Replace("\\(", "(")
                    .Replace("\\)", ")")
                    .Replace("\\\\", "\\");
            }
        }
    }
}
