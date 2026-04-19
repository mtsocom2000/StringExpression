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
                    throw new ParserException("Parser Error", i, expressionStr);
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
            string functionName = String.Empty;

            var activeExpr = new Expression();
            for (var i = 0; i < expressionStr.Length; i++)
            {
                var nextState = IsExpressionStateTransferValid(activeExpr, numericStr, functionName, exprParserState, expressionStr, i);

                if (nextState.updatedState != ExpressionParserState.eEnd)
                {
                    if (!nextState.isValid)
                    {
                        throw new ParserException("Parser Error", i, expressionStr);
                    }
                }

                activeExpr = nextState.updatedExpr;
                numericStr = nextState.updatedNumericStr;
                functionName = nextState.updatedFunctionName;
                exprParserState = nextState.updatedState;
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
            eExpectFunctionArg      = 8,        // Function argument (inside parentheses)
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

        private (bool isValid, ExpressionParserState updatedState, Expression updatedExpr, int updateIndex, string updatedNumericStr, string updatedFunctionName) 
            IsExpressionStateTransferValid(Expression expr, string currentNumericStr, string currentFunctionName, ExpressionParserState currentState, string expressionStr, int currentIndex)
        {
            bool isValid = false;
            ExpressionParserState updateState = currentState;
            Expression updatedExpr = expr;
            var updateIndex = currentIndex;
            string numericStr = currentNumericStr;
            string functionName = currentFunctionName;
            Stack<char> bracketStack = new Stack<char>();
            NumericParserState numericParserState = NumericParserState.eEmpty;
            
            for (var i = currentIndex; i < expressionStr.Length; i++)
            {
                var incomingChar = expressionStr[i];
                if (string.IsNullOrWhiteSpace(incomingChar.ToString()))
                {
                    continue;
                }

                // Handle function name accumulation
                if (currentState == ExpressionParserState.eExpectLeftExpression && Util.IsAlpha(incomingChar))
                {
                    functionName = functionName + incomingChar.ToString();
                    updateIndex = i;
                    continue;
                }
                
                // Check if we have a function call (functionName followed by '(')
                if (!string.IsNullOrEmpty(functionName) && Util.IsLeftBracket(incomingChar))
                {
                    var funcOp = OperatorFactory.Instance.Support(functionName.ToLower());
                    if (funcOp != null && funcOp is FunctionOperator)
                    {
                        // We have a valid function call - parse the argument(s)
                        bracketStack.Push(incomingChar);
                        
                        // Create a new expression with the function operator
                        var funcExpr = new Expression();
                        funcExpr.Operator = funcOp;
                        funcExpr.BracketState = BracketState.eLeft;
                        
                        // Link it properly to the parent expression
                        if (expr.IsValid || expr.BracketState == BracketState.eLeft)
                        {
                            // This shouldn't happen in normal flow, but handle it
                            expr.Left = funcExpr;
                        }
                        else
                        {
                            // Set the function expression as the current expression's value/left
                            expr.Value = null;
                            expr.Left = funcExpr;
                        }
                        
                        // Parse the argument expression inside the parentheses
                        int argStartIndex = i + 1;
                        int argEndIndex = FindMatchingBracket(expressionStr, argStartIndex, incomingChar);
                        
                        if (argEndIndex < 0)
                        {
                            throw new ParserException($"Unmatched left bracket for function '{functionName}'", i, expressionStr);
                        }
                        
                        // Extract and parse the argument(s)
                        string argStr = expressionStr.Substring(argStartIndex, argEndIndex - argStartIndex);
                        
                        // Handle multi-argument functions (like pow(x, y))
                        var funcOperator = (FunctionOperator)funcOp;
                        if (funcOperator.ArgumentCount == 2)
                        {
                            // Split by comma and parse each argument
                            var argParts = SplitFunctionArguments(argStr);
                            if (argParts.Length != 2)
                            {
                                throw new ParserException($"Function '{functionName}' requires 2 arguments, got {argParts.Length}", i, expressionStr);
                            }
                            
                            var arg1Expr = ParseToExpr(argParts[0]);
                            var arg2Expr = ParseToExpr(argParts[1]);
                            funcExpr.Left = arg1Expr;
                            funcExpr.Right = arg2Expr;
                        }
                        else
                        {
                            // Single argument function
                            if (!string.IsNullOrWhiteSpace(argStr))
                            {
                                var argExpr = ParseToExpr(argStr);
                                funcExpr.Left = argExpr;
                            }
                        }
                        
                        funcExpr.BracketState = BracketState.eClosed;
                        
                        // Update state and continue after the closing bracket
                        expr = funcExpr;
                        updatedExpr = expr;
                        i = argEndIndex;
                        updateIndex = argEndIndex;
                        functionName = String.Empty;
                        currentState = ExpressionParserState.eExpectOperator;
                        continue;
                    }
                    else
                    {
                        // Unknown function name - treat as error
                        throw new ParserException($"Unknown function '{functionName}'", i - functionName.Length, expressionStr, functionName);
                    }
                }
                
                // If we accumulated function name but next char is not '(' - invalid
                if (!string.IsNullOrEmpty(functionName) && !Util.IsFunctionNameChar(incomingChar) && !Util.IsLeftBracket(incomingChar))
                {
                    throw new ParserException($"Invalid identifier '{functionName}' - expected '(' for function call", i - functionName.Length, expressionStr, functionName);
                }

                // Handle left bracket: push to stack and create nested expression
                if (Util.IsLeftBracket(incomingChar))
                {
                    bracketStack.Push(incomingChar);
                    if (currentState == ExpressionParserState.eExpectLeftExpression)
                    {
                        if (expr.BracketState == BracketState.eLeft)
                        {
                            var newExpr = new Expression();
                            expr.Left = newExpr;
                            expr = newExpr;
                            updatedExpr = newExpr;
                        }
                        else if (expr.BracketState != BracketState.eNone)
                        {
                            throw new ParserException("Expect left bracket but found non-left bracket", i, expressionStr);
                        }
                        expr.BracketState = BracketState.eLeft;
                        updateIndex = i;
                        continue;
                    }
                    else if (currentState == ExpressionParserState.eExpectRightExpression)
                    {
                        var newExpr = new Expression();
                        newExpr.BracketState = BracketState.eLeft;
                        expr.Right = newExpr;
                        expr = newExpr;
                        updatedExpr = newExpr;
                        currentState = ExpressionParserState.eExpectLeftExpression;
                        updateIndex = i;
                        continue;
                    }
                    else
                    {
                        throw new ParserException("Expect operator but found bracket", i, expressionStr);
                    }
                }
                // Handle right bracket: check matching and close expression
                else if (Util.IsRightBracket(incomingChar))
                {
                    if (bracketStack.Count == 0)
                    {
                        throw new ParserException($"Unmatched right bracket '{incomingChar}'", i, expressionStr);
                    }
                    
                    char expectedLeftBracket = bracketStack.Pop();
                    if (!Util.BracketsMatch(expectedLeftBracket, incomingChar))
                    {
                        throw new ParserException($"Bracket mismatch: expected '{Util.GetMatchingRightBracket(expectedLeftBracket)}' but found '{incomingChar}'", i, expressionStr);
                    }
                    
                    while (expr != null && expr.BracketState != BracketState.eLeft)
                    {
                        expr = expr.Parent;
                    }
                    if (expr == null || (expr.BracketState != BracketState.eLeft))
                    {
                        throw new ParserException($"Cannot find matching left bracket for '{incomingChar}'", i, expressionStr);
                    }
                    expr.BracketState = BracketState.eClosed;
                    updatedExpr = expr;
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
                            throw new ParserException("Parser Error", i, expressionStr);
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

                // Handle comma separator for multi-argument functions
                if (Util.IsComma(incomingChar) && currentState == ExpressionParserState.eExpectOperator)
                {
                    // Comma is used as argument separator in functions
                    // For now, we treat it as ending the current argument expression
                    updateIndex = i;
                    isValid = true;
                    return (isValid, currentState, updatedExpr, updateIndex, numericStr, functionName);
                }

                if (currentState == ExpressionParserState.eExpectOperator)
                {
                    var op = OperatorFactory.Instance.Support(incomingChar.ToString());
                    if (op != null)
                    {
                        if (expr.IsValid || expr.BracketState == BracketState.eLeft)
                        {
                            if (expr.State != ExpressionState.eValueOnly)
                            {
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
                                        sourceExprHasLeftBracket = true;
                                    }

                                    if (expr.Operator.Priority == op.Priority)
                                    {
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
                                        while (expr.BracketState != BracketState.eLeft && expr.Parent != null)
                                        {
                                            expr = expr.Parent;
                                            if (expr.Operator.Priority <= op.Priority)
                                            {
                                                break;
                                            }
                                        }
                                        if (expr.BracketState == BracketState.eLeft)
                                        {
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
                                        sourceExprHasLeftBracket = false;
                                        newExpr.Left = expr.Right;
                                        expr.Right = newExpr;
                                        expr = newExpr;
                                    }
                                    else
                                    {
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
                            System.Diagnostics.Debug.Assert(false);
                        }
                        expr.Operator = op;
                        currentState = ExpressionParserState.eExpectRightExpression;
                        numericStr = String.Empty;
                        functionName = String.Empty;
                        numericParserState = NumericParserState.eEmpty;
                    }
                }
                else if (currentState == ExpressionParserState.eExpectRightExpression)
                {
                    var nextState = IsNumbericStateTransferValid(numericStr, numericParserState, incomingChar);
                    if (nextState.updatedState != NumericParserState.eEnd)
                    {
                        if (!nextState.isValid)
                        {
                            throw new ParserException("Parser Error", i, expressionStr);
                        }
                        numericParserState = nextState.updatedState;
                        numericStr = nextState.updatedNumericStr;
                        expr.Right = new Expression(numericStr);
                    }
                    else
                    {
                        currentState = ExpressionParserState.eExpectOperator;
                        i--;
                    }
                }

                updateIndex = i;
            }

            isValid = expr.IsValid;
            
            // Check if all brackets have been properly closed
            if (bracketStack.Count > 0)
            {
                char unmatchedBracket = bracketStack.Peek();
                throw new ParserException($"Unmatched left bracket '{unmatchedBracket}' - expression has {bracketStack.Count} unclosed bracket(s)", updateIndex, expressionStr);
            }
            
            return (isValid, ExpressionParserState.eEnd, updatedExpr, updateIndex, numericStr, functionName);
        }

        /// <summary>
        /// Find the matching closing bracket position
        /// </summary>
        private int FindMatchingBracket(string expressionStr, int startIndex, char openBracket)
        {
            char closeBracket = Util.GetMatchingRightBracket(openBracket);
            int depth = 1;
            
            for (int i = startIndex; i < expressionStr.Length; i++)
            {
                char c = expressionStr[i];
                if (c == openBracket)
                {
                    depth++;
                }
                else if (c == closeBracket)
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }
            
            return -1; // No matching bracket found
        }

        /// <summary>
        /// Split function arguments by comma, handling nested expressions
        /// </summary>
        private string[] SplitFunctionArguments(string argStr)
        {
            var result = new List<string>();
            int start = 0;
            int depth = 0;
            
            for (int i = 0; i < argStr.Length; i++)
            {
                char c = argStr[i];
                if (Util.IsLeftBracket(c))
                {
                    depth++;
                }
                else if (Util.IsRightBracket(c))
                {
                    depth--;
                }
                else if (c == ',' && depth == 0)
                {
                    result.Add(argStr.Substring(start, i - start).Trim());
                    start = i + 1;
                }
            }
            
            // Add the last argument
            result.Add(argStr.Substring(start).Trim());
            
            return result.ToArray();
        }

        /// <summary>
        /// Try to add incoming char to current numeric string
        /// </summary>
        private (bool isValid, string updatedNumericStr, NumericParserState updatedState) IsNumbericStateTransferValid(string currentNumericStr, NumericParserState currentState, char incomingChar)
        {
            bool isValid = false;
            string updatedNumericStr = currentNumericStr;
            NumericParserState updatedState = currentState;
            
            bool isNumeric = Util.IsNumeric(incomingChar);
            bool isPoint = Util.IsPoint(incomingChar);
            bool isSign = Util.IsSign(incomingChar);

            if (currentState == NumericParserState.eEmpty)
            {
                if (isNumeric)
                {
                    updatedState = NumericParserState.eHasNumeric;
                    isValid = true;
                    updatedNumericStr = updatedNumericStr + incomingChar.ToString();
                }
                else if (isPoint)
                {
                    updatedState = NumericParserState.eHasPoint;
                    isValid = true;
                    if (string.IsNullOrEmpty(currentNumericStr))
                    {
                        updatedNumericStr = "0" + incomingChar.ToString();
                    }
                    else
                    {
                        updatedNumericStr = currentNumericStr + incomingChar.ToString();
                    }
                }
                else if (isSign)
                {
                    updatedState = NumericParserState.eHasSign;
                    isValid = true;
                    updatedNumericStr = incomingChar.ToString();
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
                    allowSign = false;
                }
                if ((currentState & NumericParserState.eHasPoint) == NumericParserState.eHasPoint)
                {
                    allowSign = false;
                    allowPoint = false;
                }
                if ((currentState & NumericParserState.eHasNumeric) == NumericParserState.eHasNumeric)
                {
                    allowSign = false;
                }

                if (isNumeric)
                {
                    updatedNumericStr = currentNumericStr + incomingChar.ToString();
                    updatedState = updatedState | NumericParserState.eHasNumeric;
                    isValid = true;
                }
                else if (isPoint && allowPoint)
                {
                    updatedNumericStr = currentNumericStr + incomingChar.ToString();
                    updatedState = updatedState | NumericParserState.eHasPoint;
                    isValid = true;
                }
                else if (isSign)
                {
                    if (allowSign)
                    {
                        updatedNumericStr = currentNumericStr + incomingChar.ToString();
                        updatedState = updatedState | NumericParserState.eHasSign;
                        isValid = true;
                    }
                    else if ((currentState & NumericParserState.eHasNumeric) == NumericParserState.eHasNumeric)
                    {
                        if (updatedNumericStr[updatedNumericStr.Length - 1] != '.')
                        {
                            updatedState = NumericParserState.eEnd;
                        }
                    }
                }

                if (!isNumeric && !isPoint && (updatedState & NumericParserState.eHasNumeric) == NumericParserState.eHasNumeric)
                {
                    if (updatedNumericStr[updatedNumericStr.Length - 1] != '.')
                    {
                        updatedState = NumericParserState.eEnd;
                    }
                }
            }

            return (isValid, updatedNumericStr, updatedState);
        }
    }
}