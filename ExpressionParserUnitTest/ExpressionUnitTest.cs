using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ExpressionParser;

namespace ExpressionParserUnitTest
{
    [TestClass]
    public class ExpressionUnitTest
    {
        [TestMethod]
        public void Test_ValueExpression()
        {
            Expression e1 = new Expression();
            Assert.IsTrue(e1.CalcValue() == "0");

            Expression e2 = new Expression(9);
            Assert.IsTrue(e2.CalcValue() == "9");

            Expression e3 = new Expression(3.1415926);
            Assert.IsTrue(e3.CalcValue() == "3.1415926");
        }

        [TestMethod]
        public void Test_SimpleExpression_One_Operator_Two_Values()
        {
            Expression e1 = new Expression();
            e1.Left = new Expression(1);
            e1.Right = new Expression(2);
            e1.Operator = new OperatorAdd();
            Assert.IsTrue(e1.CalcValue() == "3");

            Expression e2 = new Expression(10); // set a default but set left expression will clear it
            e2.Left = new Expression(9);
            e2.Right = new Expression(18);
            e2.Operator = new OperatorSub();
            Assert.IsTrue(e2.CalcValue() == "-9");
        }

        [TestMethod]
        public void Test_SimpleExpression_Multiple_Operators_Sample_Op_Order()
        {
            Expression e1 = new Expression();
            e1.Left = new Expression(1);
            e1.Right = new Expression(2);
            e1.Operator = new OperatorAdd();    // 1 + 2

            Expression e2 = new Expression(10); // set a default but set left expression will clear it
            e2.Left = new Expression(9);
            e2.Right = new Expression(3);
            e2.Operator = new OperatorSub();    // 9 - 3

            Expression eRoot1 = new Expression();
            eRoot1.Left = e1;
            eRoot1.Right = e2;
            eRoot1.Operator = new OperatorAdd();    // 1 + 2 + 9 - 3, actually this is (1+2) + (9-3)
            Assert.IsTrue(eRoot1.CalcValue() == "9");

            Expression eAdd2 = new Expression();
            eAdd2.Left = e1;                        // 1 + 2
            eAdd2.Right = new Expression(9);
            eAdd2.Operator = new OperatorAdd();     // 1 + 2 + 9
            Expression eRoot2 = new Expression();
            eRoot2.Left = eAdd2;                    // 1 + 2 + 9
            eRoot2.Right = new Expression(3);
            eRoot2.Operator = new OperatorSub();    // 1 + 2 + 9 - 3, actually this is ((1+2) + 9) -3, extract origional calculator order
            Assert.IsTrue(eRoot2.CalcValue() == "9");
        }

        [TestMethod]
        public void Test_SimpleExpression_Multiple_Operators_Different_Op_Order()
        {
            // (1 + 2) * (9 -3)
            {
                Expression e1 = new Expression();
                e1.Left = new Expression(1);
                e1.Right = new Expression(2);
                e1.Operator = new OperatorAdd();    // 1 + 2

                Expression e2 = new Expression(); // set a default but set left expression will clear it
                e2.Left = new Expression(9);
                e2.Right = new Expression(3);
                e2.Operator = new OperatorSub();    // 9 - 3

                Expression eRoot1 = new Expression();
                eRoot1.Left = e1;
                eRoot1.Right = e2;
                eRoot1.Operator = new OperatorMultiple();    // (1 + 2) * (9 - 3)
                Assert.IsTrue(eRoot1.CalcValue() == "18");
            }

            // 1 + 2 * 9 - 3
            {
                Expression e1 = new Expression();
                e1.Left = new Expression(2);
                e1.Right = new Expression(9);
                e1.Operator = new OperatorMultiple();    // 2 * 9

                Expression e2 = new Expression();
                e2.Left = new Expression(1);
                e2.Right = e1;
                e2.Operator = new OperatorAdd();    // 1 + 2 * 9

                Expression eRoot1 = new Expression();
                eRoot1.Left = e2;
                eRoot1.Right = new Expression(3);
                eRoot1.Operator = new OperatorSub();    // 1 + 2 * 9 - 3
                Assert.IsTrue(eRoot1.CalcValue() == "16");
            }

            // 1 + 2 * (9 - 3)
            {
                Expression e1 = new Expression();
                e1.Left = new Expression(9);
                e1.Right = new Expression(3);
                e1.Operator = new OperatorSub();    // 9 - 3

                Expression e2 = new Expression();
                e2.Left = new Expression(2);
                e2.Right = e1;
                e2.Operator = new OperatorMultiple();    // 2 * (9 - 3)

                Expression eRoot1 = new Expression();
                eRoot1.Left = new Expression(1);
                eRoot1.Right = e2;
                eRoot1.Operator = new OperatorAdd();    // 1 + 2 * (9 - 3)
                Assert.IsTrue(eRoot1.CalcValue() == "13");
            }

        }
    }
}
