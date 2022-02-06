using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    public enum OperatorType
    {
        Unary = 0,
        Binrary = 1,
    }
    public class Operator
    {
        public virtual bool Support(string opStr)
        {
            return false;
        }
        public virtual string Calc(Expression expr)
        {
            return "0";
        }

        public virtual int Priority
        {
            get { return 0; }
        }

        public override string ToString()
        {
            return "";
        }
    }

    public class UnaryOperator : Operator
    {
    }

    public class BinraryOperator : Operator
    {
    }

    public class OperatorAdd : BinraryOperator
    {
        public override bool Support(string opStr)
        {
            return opStr == "+";
        }
        public override string Calc(Expression expr)
        {
            // TODO
            var v = double.Parse(expr.Left.CalcValue()) + double.Parse(expr.Right.CalcValue());
            return v.ToString();
        }
        public override int Priority
        {
            get { return 0; }
        }
        public override string ToString()
        {
            return "+";
        }
    }

    public class OperatorSub : BinraryOperator
    {
        public override bool Support(string opStr)
        {
            return opStr == "-";
        }
        public override string Calc(Expression expr)
        {
            // TODO
            var v = double.Parse(expr.Left.CalcValue()) - double.Parse(expr.Right.CalcValue());
            return v.ToString();
        }
        public override int Priority
        {
            get { return 0; }
        }
        public override string ToString()
        {
            return "-";
        }
    }

    public class OperatorMultiple : BinraryOperator
    {
        public override bool Support(string opStr)
        {
            return opStr == "*";
        }
        public override string Calc(Expression expr)
        {
            // TODO
            var v = double.Parse(expr.Left.CalcValue()) * double.Parse(expr.Right.CalcValue());
            return v.ToString();
        }
        public override int Priority
        {
            get { return 1; }
        }
        public override string ToString()
        {
            return "*";
        }
    }

    public class OperatorDivide : BinraryOperator
    {
        public override bool Support(string opStr)
        {
            return opStr == "/";
        }
        public override string Calc(Expression expr)
        {
            // TODO
            var v = double.Parse(expr.Left.CalcValue()) / double.Parse(expr.Right.CalcValue());
            return v.ToString();
        }
        public override int Priority
        {
            get { return 1; }
        }
        public override string ToString()
        {
            return "/";
        }
    }

    public class OperatorBracket : Operator
    {
        public override bool Support(string opStr)
        {
            throw new NotImplementedException();
        }
        public override string Calc(Expression expr)
        {
            return expr.Left.CalcValue();
        }
        public override int Priority
        {
            get { return 0xFF; }
        }
        public override string ToString()
        {
            return "({0})";
        }
    }
}
