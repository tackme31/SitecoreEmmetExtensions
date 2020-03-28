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
            var expected = "<input checked=\"\" type=\"checkbox\">";
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
