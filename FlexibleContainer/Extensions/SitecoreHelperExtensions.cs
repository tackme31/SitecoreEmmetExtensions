﻿using FlexibleContainer.Renderer;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using System.Text.RegularExpressions;
using System.Web;

namespace FlexibleContainer.Extensions
{
    public static class SitecoreHelperExtensions
    {
        private static readonly Regex StaticPlaceholderRegex = new Regex(@"^\[(?<placeholderKey>[^}]*)\]$");
        private static readonly Regex DynamicPlaceholderRegex = new Regex(@"^@\[(?<placeholderKey>[^}|]*?)(\|count:(?<count>\d+?))?(\|maxCount:(?<maxCount>\d+?))?(\|seed:(?<seed>\d+?))?}\]$");

        public static HtmlString RenderFlexibleContainer(this SitecoreHelper helper)
        {
            Assert.ArgumentNotNull(helper, nameof(helper));

            var parameterValue = RenderingContext.Current.Rendering.Parameters["Expression"];
            var expression = string.IsNullOrWhiteSpace(parameterValue) ? "div{[container]}" : parameterValue;
            var result = ExpressionRenderer.Render(expression, contentFormatter);
            return new HtmlString(result);

            string contentFormatter(string content)
            {
                var dynamicPlaceholderMatch = DynamicPlaceholderRegex.Match(content);
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
                    return content.Replace(dynamicPlaceholderMatch.Value, placeholder);
                }

                var staticPlaceholderMatch = StaticPlaceholderRegex.Match(content);
                if (staticPlaceholderMatch.Success)
                {
                    var placeholderKey = staticPlaceholderMatch.Groups["placeholderKey"].Value;
                    var placeholder = helper.Placeholder(placeholderKey).ToString();
                    return content.Replace(staticPlaceholderMatch.Value, placeholder);
                }

                return content;
            }
        }
    }
}