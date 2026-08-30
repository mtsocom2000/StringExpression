using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using ExpressionParser;

namespace ExpressionParserUnitTest
{
    /// <summary>
    /// Characterization / regression tests for the defects documented in
    /// docs/expression-parser-improvement-plan.md (function-call cluster F-1..F-4,
    /// unary minus Bug #2/#3, comma Bug #4).
    ///
    /// Expected to be RED at baseline; each group turns GREEN as the matching
    /// fix phase lands. Guard tests must stay GREEN throughout.
    /// </summary>
    [TestClass]
    public class ParserBugRegressionTests
    {
        private static bool ApproximatelyEqual(string result, double expected, double tolerance = 1e-10)
        {
            if (!double.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture, out double actual))
                return false;
            return Math.Abs(actual - expected) < tolerance;
        }

        private static void AssertCalc(string expression, double expected, double tolerance = 1e-10)
        {
            var exp = ExpressionBuilder.Instance.ParseToExpr(expression);
            Assert.IsTrue(ApproximatelyEqual(exp.CalcValue(), expected, tolerance),
                $"'{expression}' expected ~{expected} but CalcValue returned '{exp.CalcValue()}'");
        }

        private static void AssertParseThrows(string expression)
        {
            try
            {
                ExpressionBuilder.Instance.ParseToExpr(expression);
                Assert.Fail($"'{expression}' should throw ParserException but parsing succeeded");
            }
            catch (ParserException)
            {
                // expected
            }
        }

        // ==================================================================
        // Guards: current working behavior - must remain green through all phases
        // ==================================================================

        [TestMethod]
        public void Guard_Precedence() => AssertCalc("1+2*3", 7);

        [TestMethod]
        public void Guard_BracketOverride() => AssertCalc("(1+2)*3", 9);

        [TestMethod]
        public void Guard_NestedBrackets() => AssertCalc("((3+5)*4)", 32);

        [TestMethod]
        public void Guard_DeepNesting() => AssertCalc("((3+1)*3+5*2-2)*2-9/(4-1)", 37);

        [TestMethod]
        public void Guard_UiCase() => AssertCalc("(30)-((1+2)*3+4)*2", 4);

        [TestMethod]
        public void Guard_SignedLiteralRightOperand() => AssertCalc("2*-3", -6);

        [TestMethod]
        public void Guard_WhitespaceTolerant() => AssertCalc("  1   +    2   ", 3);

        [TestMethod]
        public void Guard_LongMixedChain() => AssertCalc("1+2*8/4-5+6-7/7+9", 14);

        // ==================================================================
        // F-cluster: function-call integration
        // Baseline: F-1 throws "Unmatched left bracket"; after F-1 alone: NRE / other throws
        // ==================================================================

        [TestMethod]
        public void Func_TopLevel() => AssertCalc("sin(0)", 0);

        [TestMethod]
        public void Func_CompositeArg() => AssertCalc("sin(1+1)", Math.Sin(2));

        [TestMethod]
        public void Func_FollowedByPlus() => AssertCalc("sin(1)+2", Math.Sin(1) + 2);

        [TestMethod]
        public void Func_FollowedByMul() => AssertCalc("sin(1)*2", Math.Sin(1) * 2);

        [TestMethod]
        public void Func_InSingleParens() => AssertCalc("(sin(1))", Math.Sin(1));

        [TestMethod]
        public void Func_InDoubleParens() => AssertCalc("((sin(1)))", Math.Sin(1));

        [TestMethod]
        public void Func_RightOperand() => AssertCalc("2*sin(1)", 2 * Math.Sin(1));

        [TestMethod]
        public void Func_RightOperandThenPlus() => AssertCalc("2*sin(1)+3", 2 * Math.Sin(1) + 3);

        [TestMethod]
        public void Func_RightOperand_NestedParens() => AssertCalc("2*(sin(1))", 2 * Math.Sin(1));

        [TestMethod]
        public void Func_NestedCalls() => AssertCalc("sin(cos(1))", Math.Sin(Math.Cos(1)));

        [TestMethod]
        public void Func_NestedWithPow() => AssertCalc("sin(pow(2,3))", Math.Sin(8));

        [TestMethod]
        public void Func_MulPriorityAfterFunction() => AssertCalc("sqrt(4)*sqrt(9)", 6);

        [TestMethod]
        public void Func_UnknownName_Throws() => AssertParseThrows("2+s(1)");

        // ==================================================================
        // Unary minus (Bug #2 / #3)
        // Baseline: "-(2+3)" silently returns 1; "-sin(5)" fails via F-1 / drops sign
        // ==================================================================

        [TestMethod]
        public void UnaryMinus_BeforeBracket() => AssertCalc("-(2+3)", -5);

        [TestMethod]
        public void UnaryMinus_BeforeBracket_SingleOperand() => AssertCalc("-(2)", -2);

        [TestMethod]
        public void UnaryMinus_BeforeBracket_WithSpaces() => AssertCalc("-  (2+3)", -5);

        [TestMethod]
        public void UnaryMinus_BeforeFunction() => AssertCalc("-sin(5)", -Math.Sin(5));

        [TestMethod]
        public void UnaryMinus_TimesAfterBracket() => AssertCalc("-(2+3)*3", -15);

        [TestMethod]
        public void UnaryMinus_PlusAfterBracket() => AssertCalc("-(2+3)+10", 5);

        [TestMethod]
        public void UnaryMinus_RightOperand_Bracket() => AssertCalc("2*-(3)", -6);

        [TestMethod]
        public void UnaryMinus_RightOperand_Function() => AssertCalc("2*-sin(1)", -2 * Math.Sin(1));

        [TestMethod]
        public void UnaryMinus_LiteralLeft_Guard() => AssertCalc("-32", -32);

        [TestMethod]
        public void UnaryMinus_DoubleNegative_Throws() => AssertParseThrows("--(2+3)");

        // ==================================================================
        // Comma outside function arguments (Bug #4)
        // Baseline: "1,2" silently returns 1 (",2" dropped)
        // ==================================================================

        [TestMethod]
        public void Comma_OutsideFunction_Throws() => AssertParseThrows("1,2");

        // ==================================================================
        // Climb-splice bug class (found while designing the unary-minus fix;
        // empirically confirmed broken on original HEAD):
        //   2*(3)+1=8, 10-(2)+3=5, 8/(2)+2=2, 8/(2)*3=1.33
        // ==================================================================

        [TestMethod]
        public void ClimbSplice_ClosedRightBracket_PlusAfter() => AssertCalc("2*(3)+1", 7);

        [TestMethod]
        public void ClimbSplice_ClosedRightBracket_MinusThenPlus() => AssertCalc("10-(2)+3", 11);

        [TestMethod]
        public void ClimbSplice_ClosedRightBracket_DivideThenPlus() => AssertCalc("8/(2)+2", 6);

        [TestMethod]
        public void ClimbSplice_ClosedRightBracket_DivideThenMul() => AssertCalc("8/(2)*3", 12);

        [TestMethod]
        public void ClimbSplice_ClosedRightBracket_EqualPriority() => AssertCalc("2*(3)*2", 12);

        [TestMethod]
        public void ClimbSplice_HigherPriorityAfter_Guard() => AssertCalc("10-(2)*3", 4);

        // ==================================================================
        // Culture independence (H-2): parsing must not depend on the thread
        // culture (de-DE uses ',' as the decimal separator).
        // ==================================================================

        [TestMethod]
        public void Culture_DotDecimalLocale_InvariantParsing()
        {
            var original = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
                AssertCalc("1.5+1", 2.5);
                AssertCalc("sin(1.5707963267949)", 1, 1e-6);
                AssertCalc("8/(2)+2", 6);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = original;
            }
        }
    }
}
