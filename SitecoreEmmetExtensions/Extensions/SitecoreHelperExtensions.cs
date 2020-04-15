using EmmetSharp;
using EmmetSharp.Models;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SitecoreEmmetExtensions.Extensions
{
    public static class SitecoreHelperExtensions
    {
        private static readonly Regex TranslationRegex = new Regex(
            @"@(?<!\\)\((?<dictionaryKey>[^)]+?)(\|(?<parameters>[^)]+))?(?<!\\)\)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex FieldRegex = new Regex(
            @"#(?<!\\)\((?<fieldName>[^)]+?)(\|(?<parameters>[^)]+))?(?<!\\)\)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex LinkRegex = new Regex(
            @"->(?<!\\)\((?<pathOrId>[^)]+?)(\|(?<parameters>[^)]+))?(?<!\\)\)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex StaticPlaceholderRegex = new Regex(
            @"^(?<!\\)\[(?<placeholderKey>[^}]+)(\|(?<parameters>[^}]+))?(?<!\\)\]$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex DynamicPlaceholderRegex = new Regex(
            @"^@(?<!\\)\[(?<placeholderKey>[^}]+?)(\|(?<parameters>[^}]+))?(?<!\\)\]$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public static HtmlString RenderEmmetAbbreviation(this SitecoreHelper helper, string abbreviation)
        {
            Assert.ArgumentNotNull(helper, nameof(helper));
            Assert.ArgumentNotNullOrEmpty(abbreviation, nameof(abbreviation));

            var result = Emmet.Expand(abbreviation, textFormatter, escapeText: false);
            return new HtmlString(result);

            HtmlTag textFormatter(HtmlTag tag)
            {
                tag = ApplyTranslationSyntax(tag);
                tag = ApplyFieldSyntax(helper, tag);
                tag = ApplyLinkSyntax(tag);
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

        private static HtmlTag ApplyFieldSyntax(SitecoreHelper helper, HtmlTag tag)
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

                    var parameters = ParseParameters(match.Groups["parameters"].Value);
                    var fromPage = MainUtil.GetBool(parameters["fromPage"], false);
                    var source = ResolveSource(fieldName, fromPage);
                    var rawValue = MainUtil.GetBool(parameters["raw"], false);
                    if (rawValue)
                    {
                        text = text.Replace(match.Value, source.item[source.field]);
                    }
                    else
                    {
                        var editable = MainUtil.GetBool(parameters["editable"], true);
                        var field = helper.Field(source.field, source.item, new { DisableWebEdit = !editable }).ToString();
                        text = text.Replace(match.Value, field);
                    }
                }

                return text;
            }

            (string field, Item item) ResolveSource(string text, bool fromPage)
            {
                var item = fromPage
                    ? RenderingContext.Current.PageContext.Item
                    : RenderingContext.Current.Rendering.Item;
                var fieldNames = text?.Split('.') ?? Array.Empty<string>();
                if (fieldNames.Length <= 1)
                {
                    return (text, item);
                }

                foreach (var fieldName in fieldNames)
                {
                    var targetItem = ((LinkField)item.Fields[fieldName])?.TargetItem ?? ((ReferenceField)item.Fields[fieldName])?.TargetItem;
                    if (targetItem == null)
                    {
                        return (fieldName, item);
                    }

                    item = targetItem;
                }

                return (text, item);
            }
        }

        private static HtmlTag ApplyLinkSyntax(HtmlTag tag)
        {
            tag.Text = MakeUrl(tag.Text);
            tag.Attributes = tag.Attributes?.ToDictionary(kv => MakeUrl(kv.Key), e => MakeUrl(e.Value));

            return tag;

            string MakeUrl(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return text;
                }

                var matches = LinkRegex.Matches(text);
                foreach (Match match in matches)
                {
                    var pathOrId = match.Groups["pathOrId"].Value;
                    var item = Context.Database.GetItem(pathOrId);
                    var url = item == null ? "#" : LinkManager.GetItemUrl(item);
                    text = text.Replace(match.Value, url);
                }
                return text;
            }
        }

        private static HtmlTag ApplyDynamicPlaceholderSyntax(SitecoreHelper helper, HtmlTag tag)
        {
            var match = DynamicPlaceholderRegex.Match(tag.Text);
            if (match.Success)
            {
                var placeholderKey = match.Groups["placeholderKey"].Value;
                var parameters = ParseParameters(match.Groups["parameters"].Value);
                var count = MainUtil.GetInt(parameters["count"], 1);
                var maxCount = MainUtil.GetInt(parameters["maxCount"], 0);
                var seed = MainUtil.GetInt(parameters["seed"], 0);
                var placeholder = helper.DynamicPlaceholder(placeholderKey, count, maxCount, seed).ToString();
                tag.Text = DynamicPlaceholderRegex.Replace(tag.Text, placeholder);
            }

            return tag;
        }

        private static HtmlTag ApplyStaticPlaceholderSyntax(SitecoreHelper helper, HtmlTag tag)
        {
            var match = StaticPlaceholderRegex.Match(tag.Text);
            if (match.Success)
            {
                var placeholderKey = match.Groups["placeholderKey"].Value;
                var placeholder = helper.Placeholder(placeholderKey).ToString();
                tag.Text = StaticPlaceholderRegex.Replace(tag.Text, placeholder);
            }

            return tag;
        }

        private static SafeDictionary<string, string> ParseParameters(string text)
        {
            var result = new SafeDictionary<string, string>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return result;
            }

            var parameters = text?
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(parameter => parameter.Split(':'));
            foreach (var parameter in parameters)
            {
                var key = parameter[0].Trim();
                var value = parameter.Length > 1 ? parameter[1].Trim() : string.Empty;
                result[key] = value;
            }

            return result;
        }
    }
}
