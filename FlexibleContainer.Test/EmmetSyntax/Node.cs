using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test.EmmetSyntax
{
    [TestClass]
    public class Node
    {
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Node_InvaidAttributesOrder1_ShouldFormatError()
        {
            ExpressionRenderer.Render("div.class#id");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Node_InvaidAttributesOrder2_ShouldFormatError()
        {
            ExpressionRenderer.Render("div[attr=\"value\"]#id");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Node_InvaidAttributesOrder3_ShouldFormatError()
        {
            ExpressionRenderer.Render("div[attr=\"value\"].class");
        }

        [TestMethod]
        public void Node_HasAllPattern_CanParse()
        {
            var expected = "<div attr1=\"value1\" attr2=\"\" class=\"class2 class1\" id=\"id\">text</div>";
            var actual = ExpressionRenderer.Render("div#id.class1.class2[attr1=\"value1\" attr2]{text}");
            Assert.AreEqual(expected, actual);
        }
    }
}
