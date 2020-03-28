using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test.EmmetSyntax
{
    [TestClass]
    public class Text
    {
        [TestMethod]
        public void Text_WithTag_CanParse()
        {
            var expected = "<p>text</p>";
            var actual = ExpressionRenderer.Render("p{text}");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Text_WithoutTag_CanParse()
        {
            var expected = "text";
            var actual = ExpressionRenderer.Render("{text}");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Text_Sibling_CanParse()
        {
            var expected = "click <a>here</a>";
            var actual = ExpressionRenderer.Render("{click }+a{here}");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Text_WithoutTagWithId_ShouldFormatError()
        {
            ExpressionRenderer.Render("#id{text}");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Text_WithoutTagWithClass_ShouldFormatError()
        {
            ExpressionRenderer.Render(".class{text}");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Text_WithoutTagWithAttribute_ShouldFormatError()
        {
            ExpressionRenderer.Render("[attr=\"value\"]{text}");
        }
    }
}
