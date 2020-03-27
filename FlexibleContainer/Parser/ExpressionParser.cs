using FlexibleContainer.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FlexibleContainer.Parser
{
    public class ExpressionParser
    {
        private static readonly Regex NodeRegex = new Regex(
            @"^" +
            @"(?<tag>[^.#{}\[\]\s]+?)?" +
            @"(#(?<id>[^.#{}\[\]\s]+?))?" +
            @"(\.(?<class>[^.#{}\[\]\s]+?)){0,}" +
            @"(\[((?<attr>[^=.#{}\[\]\s]+(=""[^""]*"")?)\s?){0,}\])?" +
            @"({(?<text>.+)})?" +
            @"$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        public static Node Parse(string expression)
        {
            var root = CreateNode("root");
            var expressions = SplitExpressionAt(TrimParenthesis(expression), '>');
            root.Children = ParseInner(expressions);
            return root;
        }

        private static List<Node> ParseInner(List<string> expressions)
        {
            if (expressions.Count < 1)
            {
                return new List<Node>();
            }

            var firstExpression = expressions[0];
            var firstSiblings = SplitExpressionAt(TrimParenthesis(firstExpression), '+');
            if (expressions.Count == 1 && firstSiblings.Count == 1)
            {
                return new List<Node>()
                {
                    CreateNode(firstSiblings[0])
                };
            }

            var result = new List<Node>();
            foreach (var sibling in firstSiblings)
            {
                var siblingExpressions = SplitExpressionAt(TrimParenthesis(sibling), '>');
                var nodes = ParseInner(siblingExpressions);
                result.AddRange(nodes);
            }

            var restExpressions = expressions.GetRange(1, expressions.Count - 1);
            if (result.Count > 0 && restExpressions.Count > 0)
            {
                var nodes = ParseInner(restExpressions);
                var lastNode = result[result.Count - 1];
                lastNode.Children = nodes;
            }

            return result;
        }

        private static string TrimParenthesis(string value)
        {
            return value.Length > 1 && value[0] == '(' && value[value.Length - 1] == ')'
                ? value.Substring(1, value.Length - 2)
                : value;
        }

        private static Node CreateNode(string node)
        {
            var tagMatch = NodeRegex.Match(node);
            if (!tagMatch.Success)
            {
                throw new FormatException($"Invalid format of the node expression (Expression: {node})");
            }

            var tag = tagMatch.Groups["tag"].Value;
            var id = tagMatch.Groups["id"].Value;
            var classList = GetCaptureValues(tagMatch, "class");
            var attributes = GetCaptureValues(tagMatch, "attr").Select(ParseAttribute).ToDictionary(attr => attr.name, attr => attr.value);
            var text = tagMatch.Groups["text"].Value;

            // HTML tag
            if (!string.IsNullOrWhiteSpace(tag))
            {
                return new Node
                {
                    Tag = tag,
                    Id = id,
                    ClassList = classList,
                    Attributes = attributes,
                    Text = text,
                };
            }

            // Only text
            if (!string.IsNullOrWhiteSpace(text) &&
                string.IsNullOrWhiteSpace(tag) &&
                string.IsNullOrWhiteSpace(id) &&
                !classList.Any() && 
                !attributes.Any())
            {
                return new Node()
                {
                    Text = text,
                };
            }

            throw new FormatException($"Tag name is missing (Expression: {node})");

            ICollection<string> GetCaptureValues(Match m, string groupName)
            {
                return m.Groups[groupName].Captures
                    .OfType<Capture>()
                    .Select(capture => capture.Value)
                    .ToList();
            }

            (string name, string value) ParseAttribute(string raw)
            {
                var splitted = raw.Split('=');
                return splitted.Length <= 1
                    ? (splitted[0], null)
                    : (splitted[0], splitted[1].Trim('"'));
            }
        }

        private static List<string> SplitExpressionAt(string expression, char delimiter)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            var nest = 0;
            var inText = false;
            var inAttr = false;
            foreach (var character in expression)
            {
                // Update status
                switch (character)
                {
                    case '{' when !inText && !inAttr:
                        inText = true;
                        break;
                    case '}' when  inText && !inAttr:
                        inText = false;
                        break;
                    case '[' when !inText && !inAttr:
                        inAttr = true;
                        break;
                    case ']' when !inText &&  inAttr:
                        inAttr = false;
                        break;
                    case '(' when !inText && !inAttr:
                        nest++;
                        break;
                    case ')' when !inText && !inAttr:
                        nest--;
                        break;
                }

                if (character != delimiter || inText || inAttr || nest > 0)
                {
                    sb.Append(character);
                    continue;
                }

                result.Add(sb.ToString());
                sb.Clear();
            }

            if (sb.Length > 0)
            {
                result.Add(sb.ToString());
            }

            if (nest < 0)
            {
                throw new FormatException($"Too much open parenthesis (Expression: {expression})");
            }

            if (nest > 0)
            {
                throw new FormatException($"Too much close parenthesis (Expression: {expression})");
            }

            return result;
        }
    }
}
