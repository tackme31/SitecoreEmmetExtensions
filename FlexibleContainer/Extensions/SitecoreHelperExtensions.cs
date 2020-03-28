using FlexibleContainer.Renderer;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using System.Text.RegularExpressions;
using System.Web;

namespace FlexibleContainer.Extensions
{
    public static class SitecoreHelperExtensions
    {
        private static readonly Regex FieldRegex = new Regex(
            @"(?<!\\){(?<fieldName>[^}]+)(?<!\\)}",
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

                    var field = helper.Field(fieldName).ToString();
                    text = text.Replace(fieldMatch.Value, field);
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
                    .Replace("\\\\", "\\");
            }
        }
    }
}
