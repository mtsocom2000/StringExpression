using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ExpressionParser;

namespace ExpressionParserUnitTest
{
    [TestClass]
    public class FunctionUnitTest
    {
        // Helper for approximate comparison due to floating point precision
        private bool ApproximatelyEqual(string result, double expected, double tolerance = 1e-10)
        {
            if (!double.TryParse(result, out double actual))
                return false;
            return Math.Abs(actual - expected) < tolerance;
        }

        #region Sin Tests
        [TestMethod]
        public void Test_Function_Sin_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sin(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Sin_Pi_Half()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sin(1.5707963267949)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1, 1e-6));
        }

        [TestMethod]
        public void Test_Function_Sin_Negative()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sin(-1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Sin(-1)));
        }

        [TestMethod]
        public void Test_Function_Sin_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sin(1+1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Sin(2)));
        }
        #endregion

        #region Cos Tests
        [TestMethod]
        public void Test_Function_Cos_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("cos(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1));
        }

        [TestMethod]
        public void Test_Function_Cos_Pi()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("cos(3.14159265358979)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), -1, 1e-6));
        }

        [TestMethod]
        public void Test_Function_Cos_Negative()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("cos(-2)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Cos(-2)));
        }

        [TestMethod]
        public void Test_Function_Cos_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("cos(3*2)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Cos(6)));
        }
        #endregion

        #region Tan Tests
        [TestMethod]
        public void Test_Function_Tan_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("tan(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Tan_One()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("tan(1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Tan(1)));
        }

        [TestMethod]
        public void Test_Function_Tan_Negative()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("tan(-0.5)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Tan(-0.5)));
        }
        #endregion

        #region Sqrt Tests
        [TestMethod]
        public void Test_Function_Sqrt_Four()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sqrt(4)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 2));
        }

        [TestMethod]
        public void Test_Function_Sqrt_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sqrt(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Sqrt_Two()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sqrt(2)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Sqrt(2)));
        }

        [TestMethod]
        public void Test_Function_Sqrt_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sqrt(9+16)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 5));
        }

        [TestMethod]
        public void Test_Function_Sqrt_Negative_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("sqrt(-1)").CalcValue();
            });
        }
        #endregion

        #region Log Tests
        [TestMethod]
        public void Test_Function_Log_One()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("log(1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Log_E()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("log(2.71828182845905)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1, 1e-6));
        }

        [TestMethod]
        public void Test_Function_Log_Ten()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("log(10)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Log(10)));
        }

        [TestMethod]
        public void Test_Function_Log_Zero_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("log(0)").CalcValue();
            });
        }

        [TestMethod]
        public void Test_Function_Log_Negative_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("log(-5)").CalcValue();
            });
        }
        #endregion

        #region Log10 Tests
        [TestMethod]
        public void Test_Function_Log10_One()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("log10(1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Log10_Ten()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("log10(10)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1));
        }

        [TestMethod]
        public void Test_Function_Log10_Hundred()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("log10(100)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 2));
        }

        [TestMethod]
        public void Test_Function_Log10_Zero_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("log10(0)").CalcValue();
            });
        }

        [TestMethod]
        public void Test_Function_Log10_Negative_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("log10(-1)").CalcValue();
            });
        }
        #endregion

        #region Ln Tests
        [TestMethod]
        public void Test_Function_Ln_One()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("ln(1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Ln_E()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("ln(2.71828182845905)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1, 1e-6));
        }

        [TestMethod]
        public void Test_Function_Ln_Two()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("ln(2)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Log(2)));
        }

        [TestMethod]
        public void Test_Function_Ln_Zero_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("ln(0)").CalcValue();
            });
        }

        [TestMethod]
        public void Test_Function_Ln_Negative_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("ln(-10)").CalcValue();
            });
        }
        #endregion

        #region Abs Tests
        [TestMethod]
        public void Test_Function_Abs_Positive()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("abs(5)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 5));
        }

        [TestMethod]
        public void Test_Function_Abs_Negative()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("abs(-5)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 5));
        }

        [TestMethod]
        public void Test_Function_Abs_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("abs(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Abs_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("abs(3-10)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 7));
        }
        #endregion

        #region Pow Tests
        [TestMethod]
        public void Test_Function_Pow_Simple()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("pow(2,3)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 8));
        }

        [TestMethod]
        public void Test_Function_Pow_Square()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("pow(5,2)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 25));
        }

        [TestMethod]
        public void Test_Function_Pow_Zero_Exponent()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("pow(10,0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1));
        }

        [TestMethod]
        public void Test_Function_Pow_Zero_Base()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("pow(0,5)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Pow_Negative_Exponent()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("pow(2,-2)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0.25));
        }

        [TestMethod]
        public void Test_Function_Pow_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("pow(2,1+2)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 8));
        }

        [TestMethod]
        public void Test_Function_Pow_Nested()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("pow(2,pow(2,2))");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 16));
        }
        #endregion

        #region Exp Tests
        [TestMethod]
        public void Test_Function_Exp_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("exp(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1));
        }

        [TestMethod]
        public void Test_Function_Exp_One()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("exp(1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.E, 1e-10));
        }

        [TestMethod]
        public void Test_Function_Emp_Two()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("exp(2)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Exp(2)));
        }

        [TestMethod]
        public void Test_Function_Emp_Negative()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("exp(-1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1/Math.E, 1e-10));
        }
        #endregion

        #region Asin Tests
        [TestMethod]
        public void Test_Function_Asin_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("asin(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Asin_One()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("asin(1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.PI/2, 1e-10));
        }

        [TestMethod]
        public void Test_Function_Asin_Half()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("asin(0.5)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Asin(0.5)));
        }

        [TestMethod]
        public void Test_Function_Asin_Negative()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("asin(-0.5)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Asin(-0.5)));
        }

        [TestMethod]
        public void Test_Function_Asin_OutOfDomain_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("asin(2)").CalcValue();
            });
        }

        [TestMethod]
        public void Test_Function_Asin_OutOfDomain_Negative_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("asin(-2)").CalcValue();
            });
        }
        #endregion

        #region Acos Tests
        [TestMethod]
        public void Test_Function_Acos_One()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("acos(1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Acos_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("acos(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.PI/2, 1e-10));
        }

        [TestMethod]
        public void Test_Function_Acos_Half()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("acos(0.5)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Acos(0.5)));
        }

        [TestMethod]
        public void Test_Function_Acos_Negative()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("acos(-1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.PI, 1e-10));
        }

        [TestMethod]
        public void Test_Function_Acos_OutOfDomain_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("acos(1.5)").CalcValue();
            });
        }

        [TestMethod]
        public void Test_Function_Acos_OutOfDomain_Negative_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("acos(-1.5)").CalcValue();
            });
        }
        #endregion

        #region Atan Tests
        [TestMethod]
        public void Test_Function_Atan_Zero()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("atan(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_Atan_One()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("atan(1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.PI/4, 1e-10));
        }

        [TestMethod]
        public void Test_Function_Atan_Negative()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("atan(-1)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), -Math.PI/4, 1e-10));
        }

        [TestMethod]
        public void Test_Function_Atan_Large()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("atan(100)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Atan(100)));
        }
        #endregion

        #region Nested Function Tests
        [TestMethod]
        public void Test_Function_Nested_Sin_Cos()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sin(cos(0))");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Sin(Math.Cos(0))));
        }

        [TestMethod]
        public void Test_Function_Nested_Sqrt_Abs()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sqrt(abs(-16))");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 4));
        }

        [TestMethod]
        public void Test_Function_Nested_Log_Pow()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("log(pow(10,2))");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), Math.Log(100)));
        }

        [TestMethod]
        public void Test_Function_Mixed_With_Operators()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sin(0)+cos(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1));
        }

        [TestMethod]
        public void Test_Function_Complex_Expression()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("sqrt(16)+abs(-5)*2");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 14));
        }

        [TestMethod]
        public void Test_Function_With_Brackets()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("(sin(0)+1)*2");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 2));
        }
        #endregion

        #region Case Insensitivity Tests
        [TestMethod]
        public void Test_Function_Uppercase_Sin()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("SIN(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 0));
        }

        [TestMethod]
        public void Test_Function_MixedCase_Cos()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("CoS(0)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 1));
        }

        [TestMethod]
        public void Test_Function_Uppercase_Sqrt()
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr("SQRT(4)");
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), 2));
        }
        #endregion

        #region Unknown Function Tests
        [TestMethod]
        public void Test_Function_Unknown_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("unknownfunc(5)");
            });
        }

        [TestMethod]
        public void Test_Function_Typo_ShouldThrow()
        {
            Assert.ThrowsException<ParserException>(() =>
            {
                ExpressionBuilder.Instance.ParseToExpr("sinx(5)");
            });
        }
        #endregion
    }
}