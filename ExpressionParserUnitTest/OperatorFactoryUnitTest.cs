using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ExpressionParser;

namespace ExpressionParserUnitTest
{
    [TestClass]
    public class OperatorFactoryUnitTest
    {
        [TestMethod]
        public void Test_OperatorFactory_Support_Operator_String()
        {
            Assert.IsNotNull(OperatorFactory.Instance.Support("+"));
            Assert.IsNotNull(OperatorFactory.Instance.Support("-"));
            Assert.IsNotNull(OperatorFactory.Instance.Support("*"));
            Assert.IsNotNull(OperatorFactory.Instance.Support("/"));

            Assert.IsNull(OperatorFactory.Instance.Support("@"));
            Assert.IsNull(OperatorFactory.Instance.Support("("));
            Assert.IsNull(OperatorFactory.Instance.Support(")"));
            Assert.IsNull(OperatorFactory.Instance.Support("0"));
        }
    }
}
