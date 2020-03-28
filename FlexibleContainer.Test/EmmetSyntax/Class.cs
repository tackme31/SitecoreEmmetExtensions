using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test.EmmetSyntax
{
    [TestClass]
    public class Class
    {
        [TestMethod]
        public void Class_Single_CanParse()
        {
            var expected = "<div class=\"class\"></div>";
            var actual = ExpressionRenderer.Render("div.class");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Class_Multiple_CanParse()
        {
            var expected = "<div class=\"class2 class1\"></div>";
            var actual = ExpressionRenderer.Render("div.class1.class2");
            Assert.AreEqual(expected, actual);
        }
    }
}
