using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test.EmmetSyntax
{
    [TestClass]
    public class Grouping
    {
        [TestMethod]
        public void Grouping_TopLevel_CanParse()
        {
            var expected =
                "<a></a>" +
                "<p></p>";
            var actual = ExpressionRenderer.Render("(a+p)");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Grouping_SiblingAsChild_CanParse()
        {
            var expected =
                "<div>" +
                    "<a></a>" +
                    "<p></p>" +
                "</div>";
            var actual = ExpressionRenderer.Render("div>(a+p)");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Grouping_ChildInMiddleSiblings_CanParse()
        {
            var expected =
                "<p></p>" +
                "<div>" +
                    "<h1></h1>" +
                "</div>" +
                "<p></p>";
            var actual = ExpressionRenderer.Render("p+(div>h1)+p");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Grouping_MultiNested_CanParse()
        {
            var expected =
                "<p></p>" +
                "<a></a>" +
                "<span></span>" +
                "<h1></h1>";
            var actual = ExpressionRenderer.Render("((p+a)+span)+h1");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Grouping_MissingOpen_ShouldFormatError()
        {
            ExpressionRenderer.Render("(a+p))");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Grouping_MissingClose_ShouldFormatError()
        {
            ExpressionRenderer.Render("((a+p)");
        }
    }
}
