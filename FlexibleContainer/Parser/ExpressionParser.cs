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
        private static readonly Regex TagRegex = new Regex(
            @"^" +
            @"(?<tag>\S+?)" + 
            @"(?<id>#\S+?)?" +
            @"(?<class>\.\S+?){0,}" +
            @"(\[((?<attr>[^=\s]+(=""[^""]*"")?)\s?){0,}\])?" +
            @"({(?<content>.+)})?" +
            @"$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        public static Node Parse(string expression)
        {
            var root = CreateNode("root");
            return root;
        }

        private static Node ParseInner(Node parent, string expression)
        {
            // remove outer parenthesis
            if (expression[0] == '(' && expression[expression.Length - 1] == ')')
            {
                expression = expression.Substring(1, expression.Length - 2).Trim();
            }

            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new FormatException($"Expression contains one or more empty nodes (Expression: {expression})");
            }

            throw new NotImplementedException();
        }

        public static Node CreateNode(string node)
        {
            var tagMatch = TagRegex.Match(node);
            if (!tagMatch.Success)
            {
                throw new FormatException($"Invalid format of the node expression (Expression: {node})");
            }

            return new Node
            {
                Tag = tagMatch.Groups["tag"].Value,
                Id = tagMatch.Groups["id"].Value,
                ClassList = GetCaptureValues(tagMatch, "class"),
                Attributes = GetCaptureValues(tagMatch, "attr").Select(ParseAttribute).ToDictionary(attr => attr.name, attr => attr.value),
                Children = new List<Node>(),
                Content = tagMatch.Groups["content"].Value,
            };

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
            var inContent = false;
            var inAttr = false;
            foreach (var character in expression)
            {
                // Update status
                switch (character)
                {
                    case '{':
                        if (!inContent && !inAttr)
                        {
                            inContent = true;
                        }
                        break;
                    case '}':
                        if (inContent && !inAttr)
                        {
                            inContent = false;
                        }
                        break;
                    case '[':
                        if (!inAttr && !inContent)
                        {
                            inAttr = true;
                        }
                        break;
                    case ']':
                        if (inAttr && !inContent)
                        {
                            inAttr = false;
                        }
                        break;
                    case '(':
                        if (!inContent && !inAttr)
                        {
                            nest++;
                        }
                        break;
                    case ')':
                        if (!inContent && !inAttr)
                        {
                            nest--;
                        }
                        break;
                }

                if (character != delimiter || inContent || inAttr || nest > 0)
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
