using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test.EmmetSyntax
{
    [TestClass]
    public class Numbering
    {
        [TestMethod]
        public void Numbering_SinlgeTag_CanParse()
        {
            var expected =
                "<h1></h1>" +
                "<h2></h2>" +
                "<h3></h3>";
            var actual = ExpressionRenderer.Render("h$*3");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Numbering_MultipleReplacing_CanParse()
        {
            var expected =
                "<p>001 01 0001 1</p>" +
                "<p>002 02 0002 2</p>";
            var actual = ExpressionRenderer.Render("p{$$$ $$ $$$$ $}*2");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Numbering_Direction_CanParse()
        {
            var expected =
                "<p>5</p>" +
                "<p>4</p>" +
                "<p>3</p>" +
                "<p>2</p>" +
                "<p>1</p>";
            var actual = ExpressionRenderer.Render("p{$@-}*5");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Numbering_Base_CanParse()
        {
            var expected =
                "<p>3</p>" +
                "<p>4</p>" +
                "<p>5</p>" +
                "<p>6</p>" +
                "<p>7</p>";
            var actual = ExpressionRenderer.Render("p{$@3}*5");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Numbering_DirectionWithBase_CanParse()
        {
            var expected =
                "<p>7</p>" +
                "<p>6</p>" +
                "<p>5</p>" +
                "<p>4</p>" +
                "<p>3</p>";
            var actual = ExpressionRenderer.Render("p{$@-3}*5");
            Assert.AreEqual(expected, actual);
        }
    }
}
