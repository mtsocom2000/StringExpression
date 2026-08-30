using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    public enum ExpressionState
    {
        eInvalid = 0,
        eValueOnly = 1,
        eLeftValueValid = 2,
        eRightValueValid = 4,
        eOperatorValid = 8,
        eAllValid = eLeftValueValid | eRightValueValid | eOperatorValid,
    }
    public enum BracketState
    {
        eNone = 0,
        eLeft = 1,
        eRight = 2,
        eClosed = eLeft | eRight
    }
    public class Expression
    {
        private string _value = string.Empty;
        private double? _numericCache;
        protected Expression _left;
        protected Expression _right;
        protected Operator _op;
        private Expression _parent;
        private ExpressionState _state = ExpressionState.eInvalid;
        private BracketState _bracket = BracketState.eNone;
        public Expression()
        {
            _value = "0";
            _state = ExpressionState.eValueOnly;
        }

        public Expression(double v)
        {
            _value = v.ToString(CultureInfo.InvariantCulture);
            _state = ExpressionState.eValueOnly;
        }

        public Expression(string value)
        {
            _value = value;
            _state = ExpressionState.eValueOnly;
        }

        public string Value
        {
            set
            {
                _value = value;
                _numericCache = null;
            }
        }

        public bool IsValid
        {
            get
            {
                return _state == ExpressionState.eValueOnly || _state == ExpressionState.eAllValid || (BracketState == BracketState.eClosed);
            }
        }

        public virtual string CalcValue()
        {
            if (!String.IsNullOrEmpty(_value))
            {
                return _value;
            }
            if (_op == null)
            {
                // [F-4 fix] Transparent wrapper: a bracket holder whose content turned out
                // to be a single sub-expression (e.g. "(sin(1))") carries neither a value
                // nor an operator - collapse to the only child instead of null-referencing.
                // (Semantics pre-figured by the retired OperatorBracket.Calc.)
                if (_left != null && _right == null)
                {
                    return _left.CalcValue();
                }
                throw new ParserException("Invalid expression node: no value and no operator");
            }
            return _op.Calc(this);
        }

        /// <summary>
        /// [P-1] Numeric view of this node: leaves parse their value once and cache;
        /// operator nodes evaluate (recursively via CalcValue) once and cache. Operators
        /// use this instead of double.Parse(CalcValue()) string round-trips per eval.
        /// The cache is invalidated by every mutating setter below.
        /// </summary>
        public double ToNumber()
        {
            if (!_numericCache.HasValue)
            {
                _numericCache = double.Parse(CalcValue(), NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            return _numericCache.Value;
        }

        public Expression Left
        {
            get { return _left; }
            set
            {
                _value = null;
                _left = value;
                _numericCache = null;
                value.Parent = this;
                if (_state == ExpressionState.eInvalid || _state == ExpressionState.eValueOnly)
                {
                    _state = ExpressionState.eLeftValueValid;
                }
                else
                {
                    _state = _state | ExpressionState.eLeftValueValid;
                }
            }
        }

        public Expression Right
        {
            get { return _right; }
            set
            {
                _right = value;
                _numericCache = null;
                value.Parent = this;
                if (_state == ExpressionState.eInvalid || _state == ExpressionState.eValueOnly)
                {
                    _state = ExpressionState.eRightValueValid;
                }
                else
                {
                    _state = _state | ExpressionState.eRightValueValid;
                }
            }
        }

        public Operator Operator
        {
            get { return _op; }
            set
            {
                _op = value;
                _numericCache = null;
                // move _value to left if had value before
                if (!String.IsNullOrEmpty(_value))
                {
                    Left = new Expression(_value);
                    if (_bracket == BracketState.eClosed)
                    {
                        Left.BracketState = BracketState.eClosed;
                        _bracket = BracketState.eNone;
                    }
                }
                if (_state == ExpressionState.eInvalid || _state == ExpressionState.eValueOnly)
                {
                    _state = ExpressionState.eOperatorValid;
                }
                else
                {
                    _state = _state | ExpressionState.eOperatorValid;
                }
            }
        }

        public Expression Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ExpressionState State
        {
            get { return _state; }
        }

        public BracketState BracketState
        {
            get { return _bracket; }
            set
            {
                _bracket = value;
                //if (_bracket != BracketState.eNone)
                //{
                //    if (Operator == null)
                //    {
                //        Operator = new OperatorBracket();
                //    }
                //}
            }
        }

        public override string ToString()
        {
            string formatStr;
            if (_state == ExpressionState.eValueOnly)
            {
                formatStr = String.Format("{0}", _value);
            }
            else
            {
                formatStr = String.Format("{0}{1}{2}", Left, Operator, Right);
            }

            if (BracketState != BracketState.eNone)
            {
                string left = "";
                if ((BracketState & BracketState.eLeft) == BracketState.eLeft)
                {
                    left = "(";
                }
                string right = "";
                if ((BracketState & BracketState.eRight) == BracketState.eRight)
                {
                    right = ")";
                }
                return String.Format("{0}{1}{2}", left, formatStr, right);
            }
            return formatStr;
        }
    }
}
