using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ExpressionParser;

namespace ExpressionParserUnitTest
{
    [TestClass]
    public class OperatorUnitTest
    {
        [TestMethod]
        public void Test_Operator_Support_Operator_String()
        {
            OperatorAdd add = new OperatorAdd();
            Assert.IsTrue(add.Support("+"));
            OperatorSub sub = new OperatorSub();
            Assert.IsTrue(sub.Support("-"));
            OperatorMultiple multiple = new OperatorMultiple();
            Assert.IsTrue(multiple.Support("*"));
            OperatorDivide divide = new OperatorDivide();
            Assert.IsTrue(divide.Support("/"));
        }
    }
}
