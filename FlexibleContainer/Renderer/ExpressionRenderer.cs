using FlexibleContainer.Parser;
using FlexibleContainer.Parser.Models;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FlexibleContainer.Renderer
{
    public static class ExpressionRenderer
    {
        public static string Render(string expression, Func<string, string> contentFormatter = null)
        {
            Assert.ArgumentNotNullOrEmpty(expression, nameof(expression));

            var rootNode = ExpressionParser.Parse(expression);
            return RenderInner(rootNode.Children, contentFormatter); 
        }

        private static string RenderInner(IList<Node> nodes, Func<string, string> contentFormatter = null)
        {
            if (nodes == null || !nodes.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                var tag = new TagBuilder(node.Tag);

                if (node.Attributes != null)
                {
                    tag.MergeAttributes(node.Attributes);
                }

                if (node.ClassList != null)
                {
                    foreach (var @class in node.ClassList)
                    {
                        tag.AddCssClass(@class);
                    }
                }

                if (!string.IsNullOrWhiteSpace(node.Id))
                {
                    tag.Attributes.Add("id", node.Id);
                }

                sb.Append(tag.ToString(TagRenderMode.StartTag));

                if (!string.IsNullOrWhiteSpace(node.Content))
                {
                    var content = contentFormatter?.Invoke(node.Content) ?? node.Content;
                    sb.Append(content);
                }

                if (node.Children != null)
                {
                    var innerHtml = RenderInner(node.Children, contentFormatter);
                    sb.Append(innerHtml);
                }

                sb.Append(tag.ToString(TagRenderMode.EndTag));
            }

            return sb.ToString();
        }
    }
}
