using System;
using System.Collections.Generic;
using System.Globalization;
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

        public override string Symbol => Name;

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
            return string.Equals(opStr, "sin", StringComparison.OrdinalIgnoreCase);
        }
        
        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for sin: {expr.Left.CalcValue()}");
            }
            var result = Math.Sin(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for sin({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
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
            return string.Equals(opStr, "cos", StringComparison.OrdinalIgnoreCase);
        }
        
        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for cos: {expr.Left.CalcValue()}");
            }
            var result = Math.Cos(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for cos({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
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
            return string.Equals(opStr, "sqrt", StringComparison.OrdinalIgnoreCase);
        }
        
        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for sqrt: {expr.Left.CalcValue()}");
            }
            if (v < 0)
            {
                throw new ParserException($"sqrt argument cannot be negative: {v}");
            }
            var result = Math.Sqrt(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for sqrt({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
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
            return string.Equals(opStr, "log", StringComparison.OrdinalIgnoreCase);
        }
        
        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for log: {expr.Left.CalcValue()}");
            }
            if (v <= 0)
            {
                throw new ParserException($"log argument must be positive: {v}");
            }
            var result = Math.Log(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for log({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
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
            return string.Equals(opStr, "abs", StringComparison.OrdinalIgnoreCase);
        }
        
        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for abs: {expr.Left.CalcValue()}");
            }
            var result = Math.Abs(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for abs({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
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
            return string.Equals(opStr, "pow", StringComparison.OrdinalIgnoreCase);
        }
        
        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var x))
            {
                throw new ParserException($"Invalid numeric argument for pow: {expr.Left.CalcValue()}");
            }
            if (!double.TryParse(expr.Right.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
            {
                throw new ParserException($"Invalid numeric argument for pow: {expr.Right.CalcValue()}");
            }
            var result = Math.Pow(x, y);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for pow({x}, {y})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
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
            return string.Equals(opStr, "exp", StringComparison.OrdinalIgnoreCase);
        }
        
        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for exp: {expr.Left.CalcValue()}");
            }
            var result = Math.Exp(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for exp({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }
    /// <summary>
    /// Tan function - tan(x)
    /// </summary>
    public class OperatorTan : FunctionOperator
    {
        public override string Name { get { return "tan"; } }
        public override int ArgumentCount { get { return 1; } }

        public override bool Support(string opStr)
        {
            return string.Equals(opStr, "tan", StringComparison.OrdinalIgnoreCase);
        }

        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for tan: {expr.Left.CalcValue()}");
            }
            var result = Math.Tan(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for tan({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Asin function - arcsin(x), domain: [-1, 1]
    /// </summary>
    public class OperatorAsin : FunctionOperator
    {
        public override string Name { get { return "asin"; } }
        public override int ArgumentCount { get { return 1; } }

        public override bool Support(string opStr)
        {
            return string.Equals(opStr, "asin", StringComparison.OrdinalIgnoreCase);
        }

        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for asin: {expr.Left.CalcValue()}");
            }
            if (v < -1 || v > 1)
            {
                throw new ParserException($"asin argument must be in [-1, 1]: {v}");
            }
            var result = Math.Asin(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for asin({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Acos function - arccos(x), domain: [-1, 1]
    /// </summary>
    public class OperatorAcos : FunctionOperator
    {
        public override string Name { get { return "acos"; } }
        public override int ArgumentCount { get { return 1; } }

        public override bool Support(string opStr)
        {
            return string.Equals(opStr, "acos", StringComparison.OrdinalIgnoreCase);
        }

        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for acos: {expr.Left.CalcValue()}");
            }
            if (v < -1 || v > 1)
            {
                throw new ParserException($"acos argument must be in [-1, 1]: {v}");
            }
            var result = Math.Acos(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for acos({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Atan function - arctan(x)
    /// </summary>
    public class OperatorAtan : FunctionOperator
    {
        public override string Name { get { return "atan"; } }
        public override int ArgumentCount { get { return 1; } }

        public override bool Support(string opStr)
        {
            return string.Equals(opStr, "atan", StringComparison.OrdinalIgnoreCase);
        }

        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for atan: {expr.Left.CalcValue()}");
            }
            var result = Math.Atan(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for atan({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Log10 function - common logarithm (base 10)
    /// </summary>
    public class OperatorLog10 : FunctionOperator
    {
        public override string Name { get { return "log10"; } }
        public override int ArgumentCount { get { return 1; } }

        public override bool Support(string opStr)
        {
            return string.Equals(opStr, "log10", StringComparison.OrdinalIgnoreCase);
        }

        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for log10: {expr.Left.CalcValue()}");
            }
            if (v <= 0)
            {
                throw new ParserException($"log10 argument must be positive: {v}");
            }
            var result = Math.Log10(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for log10({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Ln function - natural logarithm (alias for log)
    /// </summary>
    public class OperatorLn : FunctionOperator
    {
        public override string Name { get { return "ln"; } }
        public override int ArgumentCount { get { return 1; } }

        public override bool Support(string opStr)
        {
            return string.Equals(opStr, "ln", StringComparison.OrdinalIgnoreCase);
        }

        public override string Calc(Expression expr)
        {
            if (!double.TryParse(expr.Left.CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for ln: {expr.Left.CalcValue()}");
            }
            if (v <= 0)
            {
                throw new ParserException($"ln argument must be positive: {v}");
            }
            var result = Math.Log(v);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for ln({v})");
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }
}
