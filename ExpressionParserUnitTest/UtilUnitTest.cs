using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ExpressionParser;

namespace ExpressionParserUnitTest
{
    [TestClass]
    public class UtilUnitTest
    {
        [TestMethod]
        public void Test_Util_Check_Numeric()
        {
            Assert.IsTrue(Util.IsNumeric('0'));
            Assert.IsTrue(Util.IsNumeric('1'));
            Assert.IsTrue(Util.IsNumeric('2'));
            Assert.IsTrue(Util.IsNumeric('3'));
            Assert.IsTrue(Util.IsNumeric('4'));
            Assert.IsTrue(Util.IsNumeric('5'));
            Assert.IsTrue(Util.IsNumeric('6'));
            Assert.IsTrue(Util.IsNumeric('7'));
            Assert.IsTrue(Util.IsNumeric('8'));
            Assert.IsTrue(Util.IsNumeric('9'));

            Assert.IsFalse(Util.IsNumeric('+'));
            Assert.IsFalse(Util.IsNumeric('-'));
            Assert.IsFalse(Util.IsNumeric('*'));
            Assert.IsFalse(Util.IsNumeric('/'));

            Assert.IsFalse(Util.IsNumeric('A'));
            Assert.IsFalse(Util.IsNumeric('?'));
            Assert.IsFalse(Util.IsNumeric('o'));
            Assert.IsFalse(Util.IsNumeric('p'));
        }

        [TestMethod]
        public void Test_Util_Check_Operator()
        {
            Assert.IsFalse(Util.IsOperator('0'));
            Assert.IsFalse(Util.IsOperator('1'));
            Assert.IsFalse(Util.IsOperator('2'));
            Assert.IsFalse(Util.IsOperator('3'));
            Assert.IsFalse(Util.IsOperator('4'));
            Assert.IsFalse(Util.IsOperator('5'));
            Assert.IsFalse(Util.IsOperator('6'));
            Assert.IsFalse(Util.IsOperator('7'));
            Assert.IsFalse(Util.IsOperator('8'));
            Assert.IsFalse(Util.IsOperator('9'));

            Assert.IsTrue(Util.IsOperator('+'));
            Assert.IsTrue(Util.IsOperator('-'));
            Assert.IsTrue(Util.IsOperator('*'));
            Assert.IsTrue(Util.IsOperator('/'));

            Assert.IsFalse(Util.IsOperator('A'));
            Assert.IsFalse(Util.IsOperator('?'));
            Assert.IsFalse(Util.IsOperator('o'));
            Assert.IsFalse(Util.IsOperator('p'));
        }
    }
}
