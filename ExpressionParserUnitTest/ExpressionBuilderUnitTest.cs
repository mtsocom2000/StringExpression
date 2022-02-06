using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ExpressionParser;

namespace ExpressionParserUnitTest
{
    [TestClass]
    public class ExpressionBuilderUnitTest
    {
        [TestMethod]
        public void Test_Builder_Single_Digital()
        {
            var exp = ExpressionBuilder.Instance.ParseNumeric("0");
            Assert.IsTrue(exp == "0");

            exp = ExpressionBuilder.Instance.ParseNumeric("1");
            Assert.IsTrue(exp == "1");
        }

        [TestMethod]
        public void Test_Builder_Multiple_Digitals()
        {
            var exp = ExpressionBuilder.Instance.ParseNumeric("29");
            Assert.IsTrue(exp == "29");

            exp = ExpressionBuilder.Instance.ParseNumeric("301");
            Assert.IsTrue(exp == "301");

            exp = ExpressionBuilder.Instance.ParseNumeric("007");
            Assert.IsTrue(exp == "007");
        }

        [TestMethod]
        public void Test_Builder_Point_Digitals()
        {
            var exp = ExpressionBuilder.Instance.ParseNumeric("29.06");
            Assert.IsTrue(exp == "29.06");

            exp = ExpressionBuilder.Instance.ParseNumeric("0.000007");
            Assert.IsTrue(exp == "0.000007");
        }

        [TestMethod]
        public void Test_Builder_Sign_Digitals()
        {
            var exp = ExpressionBuilder.Instance.ParseNumeric("+29.06");
            Assert.IsTrue(exp == "+29.06");

            exp = ExpressionBuilder.Instance.ParseNumeric("-0.000007");
            Assert.IsTrue(exp == "-0.000007");
        }

        [TestMethod]
        public void Test_Builder_Failed_Digitals()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseNumeric("+-");
            });

            Assert.ThrowsException<ParserException>(() => {
                ExpressionBuilder.Instance.ParseNumeric("0..");
            });

            Assert.ThrowsException<ParserException>(() => {
                ExpressionBuilder.Instance.ParseNumeric("6.-");
            });
        }

        [TestMethod]
        public void Test_Builder_Numeric_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("9");
            Assert.IsTrue(exp.CalcValue() == "9");

            exp = ExpressionBuilder.Instance.ParseToExpr("32");
            Assert.IsTrue(exp.CalcValue() == "32");

            exp = ExpressionBuilder.Instance.ParseToExpr("3298765412001");
            Assert.IsTrue(exp.CalcValue() == "3298765412001");
        }

        [TestMethod]
        public void Test_Builder_Sign_Numeric_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("+9");
            Assert.IsTrue(exp.CalcValue() == "+9");

            exp = ExpressionBuilder.Instance.ParseToExpr("-32");
            Assert.IsTrue(exp.CalcValue() == "-32");
        }

        [TestMethod]
        public void Test_Builder_Point_Numeric_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("9.3456");
            Assert.IsTrue(exp.CalcValue() == "9.3456");

            exp = ExpressionBuilder.Instance.ParseToExpr("32.0006");
            Assert.IsTrue(exp.CalcValue() == "32.0006");

            exp = ExpressionBuilder.Instance.ParseToExpr("0.123123123123123123123");
            Assert.IsTrue(exp.CalcValue() == "0.123123123123123123123");
        }

        [TestMethod]
        public void Test_Builder_Point_Sign_Numeric_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("+9.3456");
            Assert.IsTrue(exp.CalcValue() == "+9.3456");

            exp = ExpressionBuilder.Instance.ParseToExpr("-32.0006");
            Assert.IsTrue(exp.CalcValue() == "-32.0006");

            exp = ExpressionBuilder.Instance.ParseToExpr("-0.123123123123123123123");
            Assert.IsTrue(exp.CalcValue() == "-0.123123123123123123123");
        }

        [TestMethod]
        public void Test_Builder_Failed_Numeric_Expression()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("+-");
            });

            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("0..");
            });

            Assert.ThrowsException<ParserException>(() => {
                ExpressionBuilder.Instance.ParseToExpr("6.-");
            });
        }

        [TestMethod]
        public void Test_Builder_Ignore_Space()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("  1   +    2   ");
            Assert.IsTrue(exp.CalcValue() == "3");
        }

        [TestMethod]
        public void Test_Builder_Simple_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("1+2");
            Assert.IsTrue(exp.CalcValue() == "3");

            exp = ExpressionBuilder.Instance.ParseToExpr("3-4");
            Assert.IsTrue(exp.CalcValue() == "-1");

            exp = ExpressionBuilder.Instance.ParseToExpr("6*8");
            Assert.IsTrue(exp.CalcValue() == "48");

            exp = ExpressionBuilder.Instance.ParseToExpr("8/2");
            Assert.IsTrue(exp.CalcValue() == "4");
        }

        [TestMethod]
        public void Test_Builder_Simple_Two_Operators_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("1+2+3");
            Assert.IsTrue(exp.CalcValue() == "6");

            exp = ExpressionBuilder.Instance.ParseToExpr("3+9-7");
            Assert.IsTrue(exp.CalcValue() == "5");

            exp = ExpressionBuilder.Instance.ParseToExpr("33/3*6");
            Assert.IsTrue(exp.CalcValue() == "66");

            exp = ExpressionBuilder.Instance.ParseToExpr("3+9*4");
            Assert.IsTrue(exp.CalcValue() == "39");

            exp = ExpressionBuilder.Instance.ParseToExpr("3+8/4");
            Assert.IsTrue(exp.CalcValue() == "5");

            exp = ExpressionBuilder.Instance.ParseToExpr("3*8-4");
            Assert.IsTrue(exp.CalcValue() == "20");
        }

        [TestMethod]
        public void Test_Builder_Simple_Three_Operators_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("1+2+3+4");
            Assert.IsTrue(exp.CalcValue() == "10");

            exp = ExpressionBuilder.Instance.ParseToExpr("1+2*3+4");
            Assert.IsTrue(exp.CalcValue() == "11");

            exp = ExpressionBuilder.Instance.ParseToExpr("2*10*3-4");
            Assert.IsTrue(exp.CalcValue() == "56");

            exp = ExpressionBuilder.Instance.ParseToExpr("1+2*8/4");
            Assert.IsTrue(exp.CalcValue() == "5");
        }

        [TestMethod]
        public void Test_Builder_Complex_Operators_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("100-5*10-23+2*7");
            Assert.IsTrue(exp.CalcValue() == "41");

            exp = ExpressionBuilder.Instance.ParseToExpr("1+2*8/4-5+6-7/7+9");
            Assert.IsTrue(exp.CalcValue() == "14");
        }

        [TestMethod]
        public void Test_Builder_Simple_Bracket_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("(1+2)");
            Assert.IsTrue(exp.CalcValue() == "3");

            exp = ExpressionBuilder.Instance.ParseToExpr("(1+2)*3");
            Assert.IsTrue(exp.CalcValue() == "9");

            exp = ExpressionBuilder.Instance.ParseToExpr("(1+2)*(3+4)");
            Assert.IsTrue(exp.CalcValue() == "21");
        }

        [TestMethod]
        public void Test_Builder_Complex_Bracket_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("((3+5)*4)");
            Assert.IsTrue(exp.CalcValue() == "32");

            exp = ExpressionBuilder.Instance.ParseToExpr("((3+5)*4+5)");
            Assert.IsTrue(exp.CalcValue() == "37");

            exp = ExpressionBuilder.Instance.ParseToExpr("((3+1)*3+5*2-2)*2-9/(4-1)");
            Assert.IsTrue(exp.CalcValue() == "37");

            exp = ExpressionBuilder.Instance.ParseToExpr("1+2*(3+4*5)");
            Assert.IsTrue(exp.CalcValue() == "47");

            exp = ExpressionBuilder.Instance.ParseToExpr("((1+2)*3+4)*5+6");
            Assert.IsTrue(exp.CalcValue() == "71");
        }

        [TestMethod]
        public void Test_Builder_Bracket_Priority_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("(30)-((1+2)*3+4)*2");
            Assert.IsTrue(exp.CalcValue() == "4");
        }

        [TestMethod]
        public void Test_Builder_String_Format_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("1+2");
            Assert.IsTrue(exp.ToString() == "1+2");

            exp = ExpressionBuilder.Instance.ParseToExpr("(3+5)");
            Assert.IsTrue(exp.ToString() == "(3+5)");

            exp = ExpressionBuilder.Instance.ParseToExpr("((3+5)*4+5)");
            Assert.IsTrue(exp.ToString() == "((3+5)*4+5)");

            exp = ExpressionBuilder.Instance.ParseToExpr("(1+2)*(3+4)");
            Assert.IsTrue(exp.ToString() == "(1+2)*(3+4)");

            exp = ExpressionBuilder.Instance.ParseToExpr("((3+1)*3+5*2-2)*2-9/(4-1)");
            Assert.IsTrue(exp.ToString() == "((3+1)*3+5*2-2)*2-9/(4-1)");
        }
    }
}
