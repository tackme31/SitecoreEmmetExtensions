using System;
using FlexibleContainer.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexibleContainer.Test
{
    [TestClass]
    public class EmmetParserTest
    {
        [TestMethod]
        public void Child_DoubleNode_CanParse()
        {
            var expected =
                "<div>" +
                    "<p>" +
                    "</p>" +
                "</div>";
            var actual = ExpressionRenderer.Render("div>p");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Child_EmptyNode_ShouldFormatError()
        {
            ExpressionRenderer.Render("div>>p");
        }

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

        [TestMethod]
        public void Group_TopLevel_CanParse()
        {
            var expected =
                "<a></a>" +
                "<p></p>";
            var actual = ExpressionRenderer.Render("(a+p)");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Group_SiblingAsChild_CanParse()
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
        public void Group_ChildInMiddleSiblings_CanParse()
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
        public void Group_MultiNested_CanParse()
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
        public void Group_MissingOpen_ShouldFormatError()
        {
            ExpressionRenderer.Render("(a+p))");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Group_MissingClose_ShouldFormatError()
        {
            ExpressionRenderer.Render("((a+p)");
        }

        [TestMethod]
        public void Id_Single_CanParse()
        {
            var expected = "<div id=\"id\"></div>";
            var actual = ExpressionRenderer.Render("div#id");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Id_Multiple_ShouldFormatError()
        {
            ExpressionRenderer.Render("div#id1#id2");
        }

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
            var expected = "<input checked=\" \"type=\"checkbox\">";
            var actual = ExpressionRenderer.Render("input[type=\"checkbox\" checked]");
            Assert.AreEqual(expected, actual);
        }

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
        public void ComplexPattern1_CanParse()
        {
            var expected = "<div attr1=\"value1\" attr2=\"\" class=\"class2 class1\" id=\"id\">text</div>";
            var actual = ExpressionRenderer.Render("div#id.class1.class2[attr1=\"value1\" attr2]{text}");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ComplexPattern2_CanParse()
        {
            var expected =
                "<div id=\"container\">" +
                    "<div>" +
                        "<p class=\"txt1\">hello</p>" +
                    "</div>" +
                    "<input type=\"button\">Submit" +
                "</div>";
            var actual = ExpressionRenderer.Render("div#container>(div>p.txt1{hello})+input[type=\"button\"]{Submit}");
            Assert.AreEqual(expected, actual);
        }
    }
}
