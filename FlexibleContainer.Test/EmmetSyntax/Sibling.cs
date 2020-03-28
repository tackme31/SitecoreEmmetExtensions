using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test.EmmetSyntax
{
    [TestClass]
    public class Sibling
    {
        [TestMethod]
        public void Sibling_DoubleNode_CanParse()
        {
            var expected =
                "<div></div>" +
                "<p></p>";
            var actual = ExpressionRenderer.Render("div+p");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Sibling_EmptyNode_ShouldFormatError()
        {
            ExpressionRenderer.Render("div++p");
        }

        [TestMethod]
        public void Sibling_AsChild_CanParse()
        {
            var expected =
                "<p>" +
                    "<a></a>" +
                    "<span></span>" +
                "</p>";
            var actual = ExpressionRenderer.Render("p>a+span");
            Assert.AreEqual(expected, actual);
        }
    }
}
