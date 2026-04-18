using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    /// <summary>
    /// Base class for function operators that take arguments
    /// </summary>
    public abstract class FunctionOperator : Operator
    {
        public override int Priority
        {
            get { return 2; }  // Higher priority than +-*/
        }

        public override string ToString()
        {
            return Name + "()";
        }

        public abstract string Name { get; }
        
        public abstract int ArgumentCount { get; }
    }

    /// <summary>
    /// Sin function - sin(x)
    /// </summary>
    public class OperatorSin : FunctionOperator
    {
        public override string Name { get { return "sin"; } }
        public override int ArgumentCount { get { return 1; } }
        
        public override bool Support(string opStr)
        {
            return opStr.ToLower() == "sin";
        }
        
        public override string Calc(Expression expr)
        {
            var v = double.Parse(expr.Left.CalcValue());
            return Math.Sin(v).ToString();
        }
    }

    /// <summary>
    /// Cos function - cos(x)
    /// </summary>
    public class OperatorCos : FunctionOperator
    {
        public override string Name { get { return "cos"; } }
        public override int ArgumentCount { get { return 1; } }
        
        public override bool Support(string opStr)
        {
            return opStr.ToLower() == "cos";
        }
        
        public override string Calc(Expression expr)
        {
            var v = double.Parse(expr.Left.CalcValue());
            return Math.Cos(v).ToString();
        }
    }

    /// <summary>
    /// Sqrt function - sqrt(x)
    /// </summary>
    public class OperatorSqrt : FunctionOperator
    {
        public override string Name { get { return "sqrt"; } }
        public override int ArgumentCount { get { return 1; } }
        
        public override bool Support(string opStr)
        {
            return opStr.ToLower() == "sqrt";
        }
        
        public override string Calc(Expression expr)
        {
            var v = double.Parse(expr.Left.CalcValue());
            if (v < 0)
            {
                throw new ParserException($"sqrt argument cannot be negative: {v}");
            }
            return Math.Sqrt(v).ToString();
        }
    }

    /// <summary>
    /// Log function (natural log) - log(x)
    /// </summary>
    public class OperatorLog : FunctionOperator
    {
        public override string Name { get { return "log"; } }
        public override int ArgumentCount { get { return 1; } }
        
        public override bool Support(string opStr)
        {
            return opStr.ToLower() == "log";
        }
        
        public override string Calc(Expression expr)
        {
            var v = double.Parse(expr.Left.CalcValue());
            if (v <= 0)
            {
                throw new ParserException($"log argument must be positive: {v}");
            }
            return Math.Log(v).ToString();
        }
    }

    /// <summary>
    /// Abs function - abs(x)
    /// </summary>
    public class OperatorAbs : FunctionOperator
    {
        public override string Name { get { return "abs"; } }
        public override int ArgumentCount { get { return 1; } }
        
        public override bool Support(string opStr)
        {
            return opStr.ToLower() == "abs";
        }
        
        public override string Calc(Expression expr)
        {
            var v = double.Parse(expr.Left.CalcValue());
            return Math.Abs(v).ToString();
        }
    }

    /// <summary>
    /// Pow function - pow(x, y)
    /// </summary>
    public class OperatorPow : FunctionOperator
    {
        public override string Name { get { return "pow"; } }
        public override int ArgumentCount { get { return 2; } }
        
        public override bool Support(string opStr)
        {
            return opStr.ToLower() == "pow";
        }
        
        public override string Calc(Expression expr)
        {
            var x = double.Parse(expr.Left.CalcValue());
            var y = double.Parse(expr.Right.CalcValue());
            return Math.Pow(x, y).ToString();
        }
    }

    /// <summary>
    /// Exp function - exp(x) (e^x)
    /// </summary>
    public class OperatorExp : FunctionOperator
    {
        public override string Name { get { return "exp"; } }
        public override int ArgumentCount { get { return 1; } }
        
        public override bool Support(string opStr)
        {
            return opStr.ToLower() == "exp";
        }
        
        public override string Calc(Expression expr)
        {
            var v = double.Parse(expr.Left.CalcValue());
            return Math.Exp(v).ToString();
        }
    }
}