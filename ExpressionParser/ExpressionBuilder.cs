using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    public class ExpressionBuilder
    {
        private ExpressionBuilder()
        {
        }

        private static ExpressionBuilder _builder = null;
        private static object _locker = new object();

        public static ExpressionBuilder Instance
        {
            get
            {
                if (_builder == null)
                {
                    lock (_locker)
                    {
                        if (_builder == null)
                            _builder = new ExpressionBuilder();
                    }
                }
                return _builder;
            }
        }

        // internal testing
        public string ParseNumeric(string expressionStr)
        {
            NumericParserState numericParserState = NumericParserState.eEmpty;
            string numericStr = String.Empty;
            for (var i = 0; i < expressionStr.Length; i++)
            {
                var nextState = IsNumbericStateTransferValid(numericStr, numericParserState, expressionStr[i]);
                if (!nextState.isValid)
                {
                    throw new ParserException("Parser Error");
                }
                numericParserState = nextState.updatedState;
                numericStr = nextState.updatedNumericStr;

                if (numericParserState == NumericParserState.eEnd)
                {
                    break;
                }
            }

            return numericStr;
        }

        public Expression ParseToExpr(string expressionStr)
        {
            ExpressionParserState exprParserState = ExpressionParserState.eExpectLeftExpression;
            string numericStr = String.Empty;

            var activeExpr = new Expression();
            for (var i = 0; i < expressionStr.Length; i++)
            {
                var nextState = IsExpressionStateTransferValid(activeExpr, numericStr, exprParserState, expressionStr, i);

                if (nextState.updatedState != ExpressionParserState.eEnd)
                {
                    if (!nextState.isValid)
                    {
                        throw new ParserException("Parser Error");
                    }
                }

                activeExpr = nextState.updatedExpr;
                i = nextState.updateIndex;
            }

            while (activeExpr.Parent != null)
            {
                activeExpr = activeExpr.Parent;
            }

            return activeExpr;
        }

        enum ExpressionParserState
        {
            eExpectLeftExpression   = 1,        // Expression
            eExpectOperator         = 2,        // Operator
            eExpectRightExpression  = 4,        //
            eEnd                    = 0xFF      //
        }

        enum NumericParserState
        {
            eEmpty              = 1,        // Zero/Default state
            eHasNumeric         = 2,        // Numeric
            eHasSign            = 4,        // Start +- sign
            eHasPoint           = 8,        // Point
            eEnd                = 0xFF,     // Found sign or other operators
        }

        private (bool isValid, ExpressionParserState updatedState, Expression updatedExpr, int updateIndex) IsExpressionStateTransferValid(Expression expr, string currentNumericStr, ExpressionParserState currentState, string expressionStr, int currentIndex)
        {
            bool isValid = false;
            ExpressionParserState updateState = currentState;
            Expression updatedExpr = expr;
            var updateIndex = currentIndex;
            // a expression could be start with numeric, and could not be start with operators (exclude +/-)
            // so check expression case first
            string numericStr = currentNumericStr;
            int bracketCount = 0;
            NumericParserState numericParserState = NumericParserState.eEmpty;
            for (var i = currentIndex; i < expressionStr.Length; i++)
            {
                var incomingChar = expressionStr[i];
                if (string.IsNullOrWhiteSpace(incomingChar.ToString()))
                {
                    continue;
                }

                // TODO: should check if bracket is part of function (i.e. "sin(3.14 / 2)"), this will be implemented later
                // if found left bracket, just create a new expression and put it as left expr of current expr, and move active expr to the left
                // if found right bracker, just move to active expr to its parent
                // definitely should check bracket matching first
                if (Util.IsLeftBracket(incomingChar))
                {
                    // we always allow left brackets
                    bracketCount++;
                    // default is expect left expression
                    if (currentState == ExpressionParserState.eExpectLeftExpression)
                    {
                        // already have a left bracket, then move to a little deep level
                        if (expr.BracketState == BracketState.eLeft)
                        {
                            var newExpr = new Expression();
                            expr.Left = newExpr;
                            expr = newExpr;
                            updatedExpr = newExpr;
                        }
                        else if (expr.BracketState != BracketState.eNone)
                        {
                            throw new ParserException("Expect left bracket but found non-left bracket");
                        }
                        expr.BracketState = BracketState.eLeft;
                        // now active expr is left child of existng expr
                        updateIndex = i;
                        continue;
                    }
                    else if (currentState == ExpressionParserState.eExpectRightExpression)
                    {
                        // create a new expression, set bracket state as left bracket
                        // and move active expr to this new created expr
                        var newExpr = new Expression();
                        newExpr.BracketState = BracketState.eLeft;
                        // newExpr.Left = new Expression();
                        expr.Right = newExpr;
                        expr = newExpr;
                        updatedExpr = newExpr;

                        // now we are expecting left expression since we create a new expr
                        currentState = ExpressionParserState.eExpectLeftExpression;
                        updateIndex = i;
                        continue;
                    }
                    else
                    {
                        throw new ParserException("Expect operator but found bracket");
                    }
                }
                else if (Util.IsRightBracket(incomingChar))
                {
                    // we should not allow right brackets count > left brackets count
                    if (bracketCount <= 0)
                    {
                        throw new ParserException("Bracket does not match Error1");
                    }
                    while (expr != null && expr.BracketState != BracketState.eLeft)
                    {
                        expr = expr.Parent;
                    }
                    if (expr == null || (expr.BracketState != BracketState.eLeft))
                    {
                        throw new ParserException("Bracket does not match Error2");
                    }
                    bracketCount--;
                    expr.BracketState = expr.BracketState | BracketState.eRight;
                    updatedExpr = expr;
                    // bracket detection is end so update string index
                    updateIndex = i;

                    if (currentState == ExpressionParserState.eExpectRightExpression)
                    {
                        currentState = ExpressionParserState.eExpectOperator;
                    }
                    continue;
                }

                if (currentState == ExpressionParserState.eExpectLeftExpression)
                {
                    var nextState = IsNumbericStateTransferValid(numericStr, numericParserState, incomingChar);
                    if (nextState.updatedState != NumericParserState.eEnd)
                    {
                        if (!nextState.isValid)
                        {
                            throw new ParserException("Parser Error");
                        }
                        numericParserState = nextState.updatedState;
                        numericStr = nextState.updatedNumericStr;
                        expr.Value = numericStr;
                    }
                    else
                    {
                        currentState = ExpressionParserState.eExpectOperator;
                    }
                }

                // since we are changing state to operator during during numeric identification,
                // so do not use else if to check currentState == ExpressionParserState.eExpectOperator here
                if (currentState == ExpressionParserState.eExpectOperator)
                {
                    var op = OperatorFactory.Instance.Support(incomingChar.ToString());
                    if (op != null)
                    {
                        // if exp is valid we should create a new exp and set the old one as left exp
                        // else set operator directlly
                        // TODO: bracket!
                        if (expr.IsValid || expr.BracketState == BracketState.eLeft)
                        {
                            if (expr.State != ExpressionState.eValueOnly)
                            {
                                // check operator of existing expr and incoming operator to determine priority
                                // if existing wins, then create a new exp, set current one as left exp of new one
                                if (expr.BracketState == BracketState.eClosed)
                                {
                                    if (expr.Parent != null)
                                    {
                                        Expression tempExpr = expr;
                                        while (tempExpr.Parent != null)
                                        {
                                            tempExpr = tempExpr.Parent;
                                            if ((tempExpr.Operator != null && tempExpr.Operator.Priority <= op.Priority) || (tempExpr.BracketState & BracketState.eLeft) == BracketState.eLeft)
                                            {
                                                expr = tempExpr;
                                                break;
                                            }
                                        }

                                        // has a valid expression, in this case should create a new expr, set current as left and associate parent properly
                                        if (tempExpr.Operator != null)
                                        {
                                            var newExpr = new Expression();
                                            newExpr.Left = tempExpr.Right;
                                            tempExpr.Right = newExpr;
                                            expr = newExpr;
                                        }
                                    }
                                    else
                                    {
                                        var newExpr = new Expression();
                                        newExpr.Left = expr;
                                        expr = newExpr;
                                    }
                                }
                                else
                                {
                                    bool sourceExprHasLeftBracket = false;
                                    if (expr.BracketState == BracketState.eLeft)
                                    {
                                        //TODO: not sure if this works for any cases...
                                        sourceExprHasLeftBracket = true;
                                    }

                                    if (expr.Operator.Priority == op.Priority)
                                    {
                                        // 1+2*8/4, when processing "/"
                                        var newExpr = new Expression();
                                        if (expr.Parent != null)
                                        {
                                            expr.Parent.Right = newExpr;
                                        }
                                        newExpr.Left = expr;
                                        if (sourceExprHasLeftBracket)
                                        {
                                            expr.BracketState = BracketState.eNone;
                                        }
                                        expr = newExpr;
                                        if (sourceExprHasLeftBracket)
                                        {
                                            expr.BracketState = BracketState.eLeft;
                                        }
                                    }
                                    else if (expr.Operator.Priority > op.Priority)
                                    {
                                        // 100-5*10-23, when processing the second "-", 5*10 should be put as left child of new expr
                                        // but how to determine (100 - 5*10) and - 23?
                                        while (expr.BracketState != BracketState.eLeft && expr.Parent != null)
                                        {
                                            expr = expr.Parent;
                                            if (expr.Operator.Priority <= op.Priority)
                                            {
                                                break;
                                            }
                                            // TODO: need consider bracket?
                                            //if ((expr.Operator != null && expr.Operator.Priority <= op.Priority) || (expr.BracketState & BracketState.eLeft) == BracketState.eLeft)
                                            //{
                                            //    break;
                                            //}
                                        }
                                        if (expr.BracketState == BracketState.eLeft)
                                        {
                                            //TODO: not sure if this works for any cases...
                                            sourceExprHasLeftBracket = true;
                                        }
                                        var newExpr = new Expression();
                                        if (expr.Parent != null)
                                        {
                                            expr.Parent.Right = newExpr;
                                        }
                                        newExpr.Left = expr;
                                        if (sourceExprHasLeftBracket)
                                        {
                                            expr.BracketState = BracketState.eNone;
                                            newExpr.BracketState = BracketState.eLeft;
                                        }
                                        expr = newExpr;
                                    }
                                    else if (expr.Operator.Priority < op.Priority)
                                    {
                                        var newExpr = new Expression();
                                        // handle parent carefully
                                        // TODO: check if this is correct or not?
                                        //if (expr.Parent != null)
                                        //{
                                        //    expr.Parent.Right = newExpr;
                                        //}
                                        //System.Diagnostics.Debug.Assert(expr.Parent == null);
                                        //if (sourceExprHasLeftBracket)
                                        //{
                                        //    expr.BracketState = BracketState.eNone;
                                        //}
                                        // create a new expr, set it as right expr of current expr
                                        // and have the original right expr as value
                                        // to process 1 + 2 * 3, when processing "*"

                                        sourceExprHasLeftBracket = false;
                                        newExpr.Left = expr.Right;
                                        expr.Right = newExpr;
                                        expr = newExpr;
                                    }
                                    else
                                    {
                                        // if incoming wins, then move right expr of existing one as left expr of new one
                                        // and set new one as right exp
                                        var newExpr = new Expression();
                                        newExpr.Left = expr.Right;
                                        expr.Right = newExpr;
                                        if (sourceExprHasLeftBracket)
                                        {
                                            expr.BracketState = BracketState.eNone;
                                        }
                                        expr = newExpr;
                                        if (sourceExprHasLeftBracket)
                                        {
                                            expr.BracketState = BracketState.eLeft;
                                        }
                                    }
                                }

                                updatedExpr = expr;
                            }
                        }
                        else
                        {
                            // TODO: throw exception?
                            System.Diagnostics.Debug.Assert(false);
                        }
                        expr.Operator = op;
                        currentState = ExpressionParserState.eExpectRightExpression;
                        // since we have operator now, we are expect left expression so clear current numeric string
                        numericStr = String.Empty;
                        numericParserState = NumericParserState.eEmpty;
                    }
                    // TODO: throw exception if cannot recorgnize operator?
                }
                else if (currentState == ExpressionParserState.eExpectRightExpression)
                {
                    var nextState = IsNumbericStateTransferValid(numericStr, numericParserState, incomingChar);
                    if (nextState.updatedState != NumericParserState.eEnd)
                    {
                        if (!nextState.isValid)
                        {
                            throw new ParserException("Parser Error");
                        }
                        numericParserState = nextState.updatedState;
                        numericStr = nextState.updatedNumericStr;
                        expr.Right = new Expression(numericStr);
                    }
                    else
                    {
                        // expression is end so should expect operator
                        // for now we do not care  bracket case
                        currentState = ExpressionParserState.eExpectOperator;
                        // move index back to let identify operator works
                        i--;
                    }
                }

                updateIndex = i;
            }

            isValid = expr.IsValid;
            return (isValid, ExpressionParserState.eEnd, updatedExpr, updateIndex);
        }

        /// <summary>
        /// Try to add incoming char to current numeric string
        /// </summary>
        /// <param name="currentNumberStr"></param>
        /// <param name="currentState"></param>
        /// <param name="incomingChar"></param>
        /// <returns>True if incomingchar is valid and </returns>
        private (bool isValid, string updatedNumericStr, NumericParserState updatedState) IsNumbericStateTransferValid(string currentNumericStr, NumericParserState currentState, char incomingChar)
        {
            bool isValid = false;
            string updatedNumericStr = currentNumericStr;
            NumericParserState updatedState = currentState;
            // default currentNumericStr is empty so default to 0
            // Check table as following to determine which input is acceptable
            // |---------------currentState------------------|--what input is acceptable----|
            // |  eEmpty  | eHasSign | eHasPoint |eHasNumeric|   Sign   |  Point  | Numeric |
            // |    V                                        |    V          V         V    |
            // |               V                             |               V         V    |
            // |                           V                 |                         V    |
            // |                                      V      |               V         V    |
            // |               V                      V      |               V         V    |
            // |                           V          V      |                         V    |
            // |               V           V                 |                         V    |
            // |               V           V          V      |                         V    |
            bool isNumeric = Util.IsNumeric(incomingChar);
            bool isPoint = Util.IsPoint(incomingChar);
            bool isSign = Util.IsSign(incomingChar);

            if (currentState == NumericParserState.eEmpty)
            {
                if (isNumeric)
                {
                    updatedState = NumericParserState.eHasNumeric;
                    isValid = true;
                    updatedNumericStr = updatedNumericStr + incomingChar.ToString();    // To number and append to existing number
                }
                else if (isPoint)
                {
                    updatedState = NumericParserState.eHasPoint;
                    isValid = true;
                    if (string.IsNullOrEmpty(currentNumericStr))
                    {
                        updatedNumericStr = "0" + incomingChar.ToString();  // To "0."
                    }
                    else
                    {
                        updatedNumericStr = currentNumericStr + incomingChar.ToString();  // To "0."
                    }
                }
                else if (isSign)
                {
                    updatedState = NumericParserState.eHasSign;
                    isValid = true;
                    updatedNumericStr = incomingChar.ToString();  // To "+" or "-"
                }
                else
                {
                    updatedState = NumericParserState.eEnd;
                }
            }
            else
            {
                bool allowSign = true;
                bool allowPoint = true;
                if ((currentState & NumericParserState.eHasSign) == NumericParserState.eHasSign)
                {
                    allowSign = false;  // Start with "+" or "-", so no "+" or "-" anymore
                }
                if ((currentState & NumericParserState.eHasPoint) == NumericParserState.eHasPoint)
                {
                    allowSign = false;  // Has point so no point anymore
                    allowPoint = false; // Has point so no "+" or "-" anymore
                }
                if ((currentState & NumericParserState.eHasNumeric) == NumericParserState.eHasNumeric)
                {
                    allowSign = false;  // Has numeric so no "+" or "-" anymore
                }

                // Run here we at least current state is valid so do not check current string is valid or not
                if (isNumeric)
                {
                    updatedNumericStr = currentNumericStr + incomingChar.ToString();  // append directlly
                    updatedState = updatedState | NumericParserState.eHasNumeric;
                    isValid = true;
                }
                else if (isPoint && allowPoint)
                {
                    updatedNumericStr = currentNumericStr + incomingChar.ToString();  // append directlly
                    updatedState = updatedState | NumericParserState.eHasPoint;
                    isValid = true;
                }
                else if (isSign)
                {
                    if (allowSign)
                    {
                        updatedNumericStr = currentNumericStr + incomingChar.ToString();  // append directlly
                        updatedState = updatedState | NumericParserState.eHasSign;
                        isValid = true;
                    }
                    else if ((currentState & NumericParserState.eHasNumeric) == NumericParserState.eHasNumeric)  // only end if we do have a numeric
                    {
                        // if end of point is still not a valid end...
                        // TODO: found a better check
                        if (updatedNumericStr[updatedNumericStr.Length - 1] != '.')
                        {
                            updatedState = NumericParserState.eEnd; // this sign should be a operator
                        }
                    }
                }

                // Run here we might found that incoming char is a sign, or a operator, or bracket, or even one letter of a function name.
                // So check if we need end numeric parsing
                if (!isNumeric && !isPoint && (updatedState & NumericParserState.eHasNumeric) == NumericParserState.eHasNumeric)
                {
                    // if end of point is still not a valid end...
                    // TODO: found a better check
                    if (updatedNumericStr[updatedNumericStr.Length - 1] != '.')
                    {
                        updatedState = NumericParserState.eEnd; // this sign should be a operator
                    }
                }
            }

            return (isValid, updatedNumericStr, updatedState);
        }

        //private bool IsOperatorStringTransferValid(Expression expr, OperatorParserState currentState, char incomingChar)
        //{
        //    return false;
        //}
    }
}
