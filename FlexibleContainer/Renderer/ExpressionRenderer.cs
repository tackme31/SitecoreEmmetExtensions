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
        public static string Render(string expression, Func<string, string> textFormatter = null)
        {
            Assert.ArgumentNotNullOrEmpty(expression, nameof(expression));

            var rootNode = ExpressionParser.Parse(expression);
            return RenderInner(rootNode.Children, textFormatter); 
        }

        private static string RenderInner(IList<Node> nodes, Func<string, string> textFormatter = null)
        {
            if (nodes == null || !nodes.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                // Text node
                if (string.IsNullOrWhiteSpace(node.Tag))
                {
                    var text = textFormatter?.Invoke(node.Text) ?? node.Text;
                    sb.Append(text);

                    var innerHtml = RenderInner(node.Children, textFormatter);
                    sb.Append(innerHtml);
                    continue;
                }

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

                if (!string.IsNullOrWhiteSpace(node.Text))
                {
                    var text = textFormatter?.Invoke(node.Text) ?? node.Text;
                    sb.Append(text);
                }

                if (node.Children != null)
                {
                    var innerHtml = RenderInner(node.Children, textFormatter);
                    sb.Append(innerHtml);
                }

                sb.Append(tag.ToString(TagRenderMode.EndTag));
            }

            return sb.ToString();
        }
    }
}
