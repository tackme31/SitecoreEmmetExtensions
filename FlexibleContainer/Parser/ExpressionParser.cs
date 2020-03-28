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
        private static readonly Regex MultiplicationRegex = new Regex(@"\*(?<multiplier>[1-9]\d*)$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex NumberingRegex = new Regex(@"(?<numbering>\$+)", RegexOptions.Compiled | RegexOptions.Singleline);

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
            var expressions = SplitExpressionAt(expression, '>');
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
            var firstSiblings = SplitExpressionAt(firstExpression, '+');
            if (!MultiplicationRegex.IsMatch(firstExpression) && expressions.Count == 1 && firstSiblings.Count == 1)
            {
                return new List<Node>()
                {
                    CreateNode(firstSiblings[0])
                };
            }

            var result = new List<Node>();
            var lastNodeMultiplir = 1;
            foreach (var sibling in firstSiblings)
            {
                // Get multiplication data
                var siblingBody = sibling;
                var multiplier = 1;
                var multiplicationMatch = MultiplicationRegex.Match(sibling);
                if (multiplicationMatch.Success)
                {
                    siblingBody = MultiplicationRegex.Replace(sibling, string.Empty);
                    multiplier = int.Parse(multiplicationMatch.Groups["multiplier"].Value);
                }

                // Multiply nodes
                for (var i = 1; i <= multiplier; i++)
                {
                    var numberedBody = ReplaceNumberings(siblingBody, i);
                    var siblingExpressions = SplitExpressionAt(numberedBody, '>');
                    var nodes = ParseInner(siblingExpressions);
                    result.AddRange(nodes);
                }

                lastNodeMultiplir = multiplier;
            }

            var restExpressions = expressions.GetRange(1, expressions.Count - 1);
            if (result.Count > 0 && restExpressions.Count > 0)
            {
                var nodes = ParseInner(restExpressions);
                var lastNodes = result.GetRange(result.Count - lastNodeMultiplir, lastNodeMultiplir);

                // When the last node is multiplied, set its children to each node.
                foreach (var lastNode in lastNodes)
                {
                    lastNode.Children = nodes;
                }
            }

            return result;
        }

        private static string ReplaceNumberings(string expression, int number)
        {
            var numberingMatches = NumberingRegex
                .Matches(expression)
                .OfType<Match>()
                .OrderByDescending(m => m.Groups["numbering"].Value.Length);
            foreach (var numberingMatch in numberingMatches)
            {
                var numbering = numberingMatch.Groups["numbering"].Value;
                var numbers = number.ToString().PadLeft(numbering.Length, '0');
                expression = expression.Replace(numbering, numbers);
            }

            return expression;
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
            foreach (var character in TrimParenthesis(expression))
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

            if (result.Any(exp => exp.Length == 0))
            {
                throw new FormatException($"An empty node is contained in the expression (Expression: {expression})");
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

            string TrimParenthesis(string value)
            {
                return value.Length > 1 && value[0] == '(' && value[value.Length - 1] == ')'
                    ? value.Substring(1, value.Length - 2)
                    : value;
            }
        }
    }
}
