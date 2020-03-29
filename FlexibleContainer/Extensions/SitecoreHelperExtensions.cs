using EmmetSharp.Models;
using EmmetSharp.Renderer;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using System.Linq;
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
            var result = AbbreviationRenderer.Render(expression, textFormatter);
            return new HtmlString(result);

            Node textFormatter(Node node)
            {
                node = ApplyTranslationSyntax(node);
                node = ApllyFieldInterpolationSyntax(helper, node);
                node = ApplyDynamicPlaceholderSyntax(helper, node);
                node = ApplyStaticPlaceholderSyntax(helper, node);
                node.Text = node.Text
                    .Replace("\\[", "[").Replace("\\]", "]")
                    .Replace("\\{", "{").Replace("\\}", "}")
                    .Replace("\\(", "(").Replace("\\)", ")")
                    .Replace("\\\\", "\\");

                return node;
            }
        }

        private static Node ApplyTranslationSyntax(Node node)
        {
            node.Text = DoTranslate(node.Text);
            node.Id = DoTranslate(node.Id);
            node.ClassList = node.ClassList?.Select(DoTranslate).ToList();
            node.Attributes = node.Attributes?.ToDictionary(kv => DoTranslate(kv.Key), e => DoTranslate(e.Value));

            return node;

            string DoTranslate(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return text;
                }

                var matches = TranslationRegex.Matches(text);
                foreach (Match match in matches)
                {
                    var dictionaryKey = match.Groups["dictionaryKey"].Value;
                    text = text.Replace(match.Value, Translate.Text(dictionaryKey));
                }
                return text;
            }
        }

        private static Node ApllyFieldInterpolationSyntax(SitecoreHelper helper, Node node)
        {
            node.Text = DoInterpolate(node.Text);
            node.Id = DoInterpolate(node.Id);
            node.ClassList = node.ClassList?.Select(DoInterpolate).ToList();
            node.Attributes = node.Attributes?.ToDictionary(kv => DoInterpolate(kv.Key), kv => DoInterpolate(kv.Value));

            return node;

            string DoInterpolate(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return text;
                }

                var matches = FieldRegex.Matches(text);
                foreach (Match match in matches)
                {
                    var fieldName = match.Groups["fieldName"].Value;
                    if (string.IsNullOrWhiteSpace(fieldName))
                    {
                        continue;
                    }

                    var editable = MainUtil.GetBool(match.Groups["editable"].Value, true);
                    var field = helper.Field(fieldName, new { DisableWebEdit = !editable }).ToString();
                    text = text.Replace(match.Value, field);
                }

                return text;
            }
        }

        private static Node ApplyDynamicPlaceholderSyntax(SitecoreHelper helper, Node node)
        {
            var dynamicPlaceholderMatch = DynamicPlaceholderRegex.Match(node.Text);
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
                node.Text = DynamicPlaceholderRegex.Replace(node.Text, placeholder);
            }

            return node;
        }

        private static Node ApplyStaticPlaceholderSyntax(SitecoreHelper helper, Node node)
        {
            var staticPlaceholderMatch = StaticPlaceholderRegex.Match(node.Text);
            if (staticPlaceholderMatch.Success)
            {
                var placeholderKey = staticPlaceholderMatch.Groups["placeholderKey"].Value;
                var placeholder = helper.Placeholder(placeholderKey).ToString();
                node.Text = StaticPlaceholderRegex.Replace(node.Text, placeholder);
            }

            return node;
        }
    }
}
