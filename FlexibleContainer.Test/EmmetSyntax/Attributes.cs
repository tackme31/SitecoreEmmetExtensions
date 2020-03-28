using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test.EmmetSyntax
{
    [TestClass]
    public class Attributes
    {
        [TestMethod]
        public void Attributes_WithValue_CanParse()
        {
            var expected = "<input type=\"checkbox\">";
            var actual = ExpressionRenderer.Render("input[type=\"checkbox\"]");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Attributes_WithoutValue_CanParse()
        {
            var expected = "<input disabled=\"\">";
            var actual = ExpressionRenderer.Render("input[disabled]");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Attributes_Multiple_CanParse()
        {
            var expected = "<input checked=\"\" type=\"checkbox\">";
            var actual = ExpressionRenderer.Render("input[type=\"checkbox\" checked]");
            Assert.AreEqual(expected, actual);
        }
    }
}
