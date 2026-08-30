using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// <summary>
        /// Canonical symbol this operator matches, or null when the operator
        /// is not reachable by symbol lookup (e.g. negate / bracket helpers).
        /// Used by OperatorFactory to build a dictionary-based fast path.
        /// </summary>
        public virtual string Symbol => null;

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
        public override string Symbol => "+";

        public override bool Support(string opStr)
        {
            return opStr == "+";
        }
        public override string Calc(Expression expr)
        {
            // TODO
            var v = expr.Left.ToNumber() + expr.Right.ToNumber();
            return v.ToString(CultureInfo.InvariantCulture);
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
        public override string Symbol => "-";

        public override bool Support(string opStr)
        {
            return opStr == "-";
        }
        public override string Calc(Expression expr)
        {
            // TODO
            var v = expr.Left.ToNumber() - expr.Right.ToNumber();
            return v.ToString(CultureInfo.InvariantCulture);
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
        public override string Symbol => "*";

        public override bool Support(string opStr)
        {
            return opStr == "*";
        }
        public override string Calc(Expression expr)
        {
            // TODO
            var v = expr.Left.ToNumber() * expr.Right.ToNumber();
            return v.ToString(CultureInfo.InvariantCulture);
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
        public override string Symbol => "/";

        public override bool Support(string opStr)
        {
            return opStr == "/";
        }
        public override string Calc(Expression expr)
        {
            var leftValue = expr.Left.ToNumber();
            var rightValue = expr.Right.ToNumber();

            // Check for division by zero
            if (rightValue == 0)
            {
                throw new ParserException("Division by zero is not allowed");
            }

            var v = leftValue / rightValue;
            return v.ToString(CultureInfo.InvariantCulture);
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

    /// <summary>
    /// [Bug #2/#3 fix] Unary minus operator. Never matched from the factory symbol
    /// scan - the parser constructs it directly when a pending bare sign is followed
    /// by '(' or a function name. The operand is stored in Right (same convention as
    /// FunctionOperator arguments). Priority 2: binds tighter than binary +- (0) and
    /// */ (1), same level as function calls. The completed "-(...)" wrapper is an
    /// atomic operand: following binary operators wrap ABOVE it (handled by the
    /// eClosed climb exhaustion wrap in ExpressionBuilder).
    /// </summary>
    public class OperatorNegate : UnaryOperator
    {
        public override bool Support(string opStr)
        {
            return false; // internal operator - never matched from input symbols
        }

        public override string Calc(Expression expr)
        {
            var rightStr = expr.Right.CalcValue();
            if (!double.TryParse(rightStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ParserException($"Invalid numeric argument for unary minus: {rightStr}");
            }
            var result = -v;
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new ParserException($"Invalid result for unary minus of {rightStr}");
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }

        public override int Priority
        {
            get { return 2; }
        }

        public override string ToString()
        {
            return "-";
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
