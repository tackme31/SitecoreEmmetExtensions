using EmmetSharp;
using EmmetSharp.Models;
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
            var result = Emmet.Expand(expression, textFormatter, escapeText: false);
            return new HtmlString(result);

            HtmlTag textFormatter(HtmlTag tag)
            {
                tag = ApplyTranslationSyntax(tag);
                tag = ApllyFieldInterpolationSyntax(helper, tag);
                tag = ApplyDynamicPlaceholderSyntax(helper, tag);
                tag = ApplyStaticPlaceholderSyntax(helper, tag);
                tag.Text = tag.Text
                    .Replace("\\[", "[").Replace("\\]", "]")
                    .Replace("\\{", "{").Replace("\\}", "}")
                    .Replace("\\(", "(").Replace("\\)", ")")
                    .Replace("\\\\", "\\");

                return tag;
            }
        }

        private static HtmlTag ApplyTranslationSyntax(HtmlTag tag)
        {
            tag.Text = DoTranslate(tag.Text);
            tag.Id = DoTranslate(tag.Id);
            tag.ClassList = tag.ClassList?.Select(DoTranslate).ToList();
            tag.Attributes = tag.Attributes?.ToDictionary(kv => DoTranslate(kv.Key), e => DoTranslate(e.Value));

            return tag;

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

        private static HtmlTag ApllyFieldInterpolationSyntax(SitecoreHelper helper, HtmlTag tag)
        {
            tag.Text = DoInterpolate(tag.Text);
            tag.Id = DoInterpolate(tag.Id);
            tag.ClassList = tag.ClassList?.Select(DoInterpolate).ToList();
            tag.Attributes = tag.Attributes?.ToDictionary(kv => DoInterpolate(kv.Key), kv => DoInterpolate(kv.Value));

            return tag;

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

        private static HtmlTag ApplyDynamicPlaceholderSyntax(SitecoreHelper helper, HtmlTag tag)
        {
            var dynamicPlaceholderMatch = DynamicPlaceholderRegex.Match(tag.Text);
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
                tag.Text = DynamicPlaceholderRegex.Replace(tag.Text, placeholder);
            }

            return tag;
        }

        private static HtmlTag ApplyStaticPlaceholderSyntax(SitecoreHelper helper, HtmlTag tag)
        {
            var staticPlaceholderMatch = StaticPlaceholderRegex.Match(tag.Text);
            if (staticPlaceholderMatch.Success)
            {
                var placeholderKey = staticPlaceholderMatch.Groups["placeholderKey"].Value;
                var placeholder = helper.Placeholder(placeholderKey).ToString();
                tag.Text = StaticPlaceholderRegex.Replace(tag.Text, placeholder);
            }

            return tag;
        }
    }
}
