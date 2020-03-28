using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test.EmmetSyntax
{
    [TestClass]
    public class Multiplication
    {
        [TestMethod]
        public void Multiplication_WithSingleTag_CanParse()
        {
            var expected =
                "<li></li>" +
                "<li></li>";
            var actual = ExpressionRenderer.Render("li*2");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Multiplication_WithSibling_CanParse()
        {
            var expected =
                "<li></li><a></a>" +
                "<li></li><a></a>";
            var actual = ExpressionRenderer.Render("(li+a)*2");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Multiplication_WithChild_CanParse()
        {
            var expected =
                "<div>" +
                    "<li></li>" +
                "</div>" +
                "<div>" +
                    "<li></li>" +
                "</div>";
            var actual = ExpressionRenderer.Render("(div>li)*2");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Multiplication_ParentBy_CanParse()
        {
            var expected =
                "<div>" +
                    "<li></li>" +
                "</div>" +
                "<div>" +
                    "<li></li>" +
                "</div>";
            var actual = ExpressionRenderer.Render("div*2>li");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Multiplication_WithNestedChild_CanParse()
        {
            var expected =
                "<div>" +
                    "<div>" +
                        "<li></li>" +
                    "</div>" +
                    "<div>" +
                        "<li></li>" +
                    "</div>" +
                "</div>";
            var actual = ExpressionRenderer.Render("div>(div>li)*2");
            Assert.AreEqual(expected, actual);
        }
    }
}
