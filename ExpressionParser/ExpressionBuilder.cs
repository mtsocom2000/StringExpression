using System;
using System.Collections.Generic;
using System.Globalization;
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
            // [P-2 fix] A digit run in right-operand position defers its Expression
            // allocation: the old code assigned expr.Right once per digit character
            // ("123" created 3 Expression objects, 2 of them garbage). The pending
            // operand is flushed exactly once when the digit run ends (operator,
            // bracket, or end of input).
            bool rightPending = false;
            
            for (var i = currentIndex; i < expressionStr.Length; i++)
            {
                var incomingChar = expressionStr[i];
                if (char.IsWhiteSpace(incomingChar))
                {
                    continue;
                }

                // Handle function name accumulation
                // [F-3 fix] Also recognize function names in right-operand position
                // (e.g. "2*sin(1)") - previously only eExpectLeftExpression did, so the
                // letters fell into the numeric FSA, were silently dropped, and the
                // following '(' then threw "Expect operator but found bracket".
                // [log10 fix] Digits continue a function name once it has started
                // ("log10" used to parse as log + number 10 -> log(10)).
                if ((currentState == ExpressionParserState.eExpectLeftExpression
                     || currentState == ExpressionParserState.eExpectRightExpression)
                    && (Util.IsAlpha(incomingChar)
                        || (!string.IsNullOrEmpty(functionName) && Util.IsNumeric(incomingChar))))
                {
                    // [guard] digits immediately before an identifier are invalid ("2sin(x)")
                    if (!string.IsNullOrEmpty(numericStr) && numericStr != "-" && numericStr != "+")
                    {
                        throw new ParserException($"Expected operator before identifier", i, expressionStr);
                    }
                    functionName = functionName + incomingChar.ToString();
                    updateIndex = i;
                    continue;
                }
                
                // Check if we have a function call (functionName followed by '(')
                if (!string.IsNullOrEmpty(functionName) && Util.IsLeftBracket(incomingChar))
                {
                    var funcOp = OperatorFactory.Instance.Support(functionName);
                    if (funcOp != null && funcOp is FunctionOperator)
                    {
                        // We have a valid function call - parse the argument(s)
                        // [F-1 fix] No bracketStack.Push here: bracket matching is already
                        // validated by FindMatchingBracket below, and the matching ')' is
                        // consumed by this branch (skipped via i = argEndIndex). Pushing here
                        // left an orphan entry that tripped the end-of-loop unmatched check.
                        // Argument-internal brackets are handled by the recursive ParseToExpr.

                        // Create a new expression with the function operator
                        var funcExpr = new Expression();
                        funcExpr.Operator = funcOp;
                        funcExpr.BracketState = BracketState.eLeft;
                        
                        // Link it properly to the parent expression
                        // [Bug #2/#3 fix] A pending bare +/- immediately before a function
                        // call is a prefix operator, not part of a number. "-sin(5)" used to
                        // lose the sign when the Left setter nulled the accumulated value.
                        // '-' wraps the call in a Negate node; '+' is identity.
                        // NOTE: attachNode may differ from funcExpr (the real call node) -
                        // argument assignment below must keep targeting funcExpr.
                        Expression attachNode = funcExpr;
                        if (numericStr == "-" || numericStr == "+")
                        {
                            bool isMinus = numericStr == "-";
                            numericStr = String.Empty;
                            numericParserState = NumericParserState.eEmpty;

                            if (isMinus)
                            {
                                funcExpr.BracketState = BracketState.eClosed; // the call itself is complete
                                var negateNode = new Expression();
                                negateNode.Value = null; // keep the Operator setter from moving a default "0" into Left
                                negateNode.Operator = new OperatorNegate();
                                negateNode.Right = funcExpr;
                                negateNode.BracketState = BracketState.eClosed; // complete operand wrapper
                                attachNode = negateNode;
                            }
                        }

                        // [F-2/F-3 fix] Attach the completed function call (possibly wrapped
                        // in a Negate node by the unary dispatch above).
                        //   - right-operand position (eExpectRightExpression): expr.Right
                        //   - untouched root placeholder: discard it, funcExpr becomes the
                        //     active node itself - a following operator then hits the
                        //     standard Parent==null wrap branch of the rotation machinery
                        //   - otherwise (bracket holder etc.): Left link; a valueless,
                        //     operatorless holder collapses transparently in
                        //     Expression.CalcValue (F-4)
                        expr = AttachOperandNode(expr, attachNode, currentState == ExpressionParserState.eExpectRightExpression);
                        
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
                        attachNode.BracketState = BracketState.eClosed;

                        // Update state and continue after the closing bracket
                        expr = attachNode;
                        updatedExpr = attachNode;
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
                    // [Bug #2 fix] Unary sign dispatch: a pending bare +/- followed by '('
                    // is a prefix operator, not part of the number inside. "-(2+3)" used to
                    // mis-bind the sign onto the first digit inside the brackets.
                    // '-' wraps the bracket sub-expression in a Negate node; '+' is identity.
                    if (numericStr == "-" || numericStr == "+")
                    {
                        bool isMinus = numericStr == "-";
                        numericStr = String.Empty;
                        numericParserState = NumericParserState.eEmpty;

                        var operandExpr = new Expression();
                        operandExpr.BracketState = BracketState.eLeft;

                        Expression attachNode = operandExpr;
                        if (isMinus)
                        {
                            var negateNode = new Expression();
                            negateNode.Value = null; // keep the Operator setter from moving a default "0" into Left
                            negateNode.Operator = new OperatorNegate();
                            negateNode.Right = operandExpr;
                            negateNode.BracketState = BracketState.eClosed; // complete operand wrapper
                            attachNode = negateNode;
                        }

                        expr = AttachOperandNode(expr, attachNode, currentState == ExpressionParserState.eExpectRightExpression);

                        bracketStack.Push(incomingChar);
                        expr = operandExpr;
                        updatedExpr = operandExpr;
                        currentState = ExpressionParserState.eExpectLeftExpression;
                        updateIndex = i;
                        continue;
                    }

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
                        // [P-2 fix] The bracket overwrites any pending digit run - the
                        // Right setter below discards it, so the flag must be cleared to
                        // keep a later end-of-loop flush from writing into newExpr.
                        rightPending = false;
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

                    // [P-2 fix] The right-bracket branch intercepts the char before the
                    // numeric FSA can see it, so "(2+3)" closes without the eEnd flush
                    // ever running. Flush the pending Right operand before closing.
                    if (rightPending)
                    {
                        expr.Right = new Expression(numericStr);
                        rightPending = false;
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
                // [Bug #4 fix] A comma can only ever appear inside function arguments,
                // and those are extracted (SplitFunctionArguments) before the main loop
                // sees them - so a comma here is always invalid input. It used to be
                // silently accepted, dropping everything after it ("1,2" -> 1).
                if (Util.IsComma(incomingChar) && currentState == ExpressionParserState.eExpectOperator)
                {
                    throw new ParserException("Comma is only valid inside function arguments", i, expressionStr);
                }

                if (currentState == ExpressionParserState.eExpectOperator)
                {
                    var op = OperatorFactory.Instance.Support(incomingChar.ToString());
                    if (op != null)
                    {
                        if (expr.IsValid || expr.BracketState == BracketState.eLeft)
                        {
                            // [climb fix] A CLOSED node is an atomic operand even when its
                            // content is a single value ("(3)"): without the eClosed escape,
                            // the Operator setter below would move "(3)"'s value into the new
                            // operator's Left, silently flattening the bracket ("2*(3)+1" -> 8).
                            if (expr.State != ExpressionState.eValueOnly || expr.BracketState == BracketState.eClosed)
                            {
                                if (expr.BracketState == BracketState.eClosed)
                                {
                                    if (expr.Parent != null)
                                    {
                                        // [climb fix] The closed sub-tree is an atomic operand. Find
                                        // the attachment point for the incoming operator by climbing
                                        // past strictly-stricter ancestors:
                                        //   - open holder (no operator yet) -> operator becomes its own
                                        //   - ancestor priority == op -> left-assoc: wrap ABOVE ancestor
                                        //   - ancestor priority <  op -> descend: take over ancestor's Right
                                        //   - exhausted (all stricter, incl. Negate wrappers) -> wrap above all
                                        // The old code only ever spliced into the parent's Right, silently
                                        // computing "2*(3)+1"=8, "10-(2)+3"=5, "8/(2)+2"=2, "8/(2)*3"=1.33
                                        // (empirically confirmed on HEAD).
                                        Expression attachAt = expr.Parent;
                                        while (attachAt != null
                                               && attachAt.BracketState != BracketState.eLeft
                                               && attachAt.Operator != null
                                               && attachAt.Operator.Priority > op.Priority)
                                        {
                                            attachAt = attachAt.Parent;
                                        }

                                        var newExpr = new Expression();
                                        if (attachAt == null)
                                        {
                                            // exhausted above the root: the whole chain becomes Left.
                                            // Find the topmost ancestor BEFORE linking - the Left setter
                                            // below rewrites expr.Parent, and walking Parent afterwards
                                            // would loop on newExpr itself.
                                            var top = expr;
                                            while (top.Parent != null)
                                            {
                                                top = top.Parent;
                                            }
                                            newExpr.Left = top;
                                            expr = newExpr;
                                        }
                                        else if (attachAt.BracketState == BracketState.eLeft && attachAt.Operator == null)
                                        {
                                            // open holder adopts the operator (parsed content stays Left)
                                            expr = attachAt;
                                        }
                                        else if (attachAt.Operator != null && attachAt.Operator.Priority == op.Priority)
                                        {
                                            // left-associative: wrap above the ancestor
                                            // (capture the old parent FIRST - the Left setter below
                                            //  rewrites attachAt.Parent, and relinking against the new
                                            //  parent would create a self-cycle and hang the parser)
                                            var oldParent = attachAt.Parent;
                                            newExpr.Left = attachAt;
                                            if (oldParent != null)
                                            {
                                                if (ReferenceEquals(oldParent.Left, attachAt))
                                                {
                                                    oldParent.Left = newExpr;
                                                }
                                                else
                                                {
                                                    oldParent.Right = newExpr;
                                                }
                                            }
                                            expr = newExpr;
                                        }
                                        else
                                        {
                                            // ancestor binds looser: the new operator descends into its
                                            // Right, taking over the completed atomic sub-tree
                                            newExpr.Left = expr;
                                            attachAt.Right = newExpr;
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
                                            if (expr.Operator != null && expr.Operator.Priority <= op.Priority)
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
                            // [H-3 fix] Debug.Assert is a no-op in Release builds and used to
                            // leave the tree in an inconsistent state - fail loudly instead.
                            throw new ParserException("Invalid expression state: operator encountered on incomplete node", i, expressionStr);
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
                        // [P-2 fix] Mark the digit run as a pending operand instead of
                        // allocating per character. A lone sign ("-" / "+") is not an
                        // operand (same guard as the identifier check above) and never
                        // becomes pending.
                        if (numericStr.Length > 0 && numericStr != "-" && numericStr != "+")
                        {
                            rightPending = true;
                        }
                    }
                    else
                    {
                        // [P-2 fix] Digit run ended (operator / bracket / comma follows):
                        // flush the completed operand once, then let the next char be
                        // re-processed in eExpectOperator.
                        if (rightPending)
                        {
                            expr.Right = new Expression(numericStr);
                            rightPending = false;
                        }
                        currentState = ExpressionParserState.eExpectOperator;
                        i--;
                    }
                }

                updateIndex = i;
            }

            // [P-2 fix] Flush a pending trailing digit run ("2+3"): the loop ends
            // without a state transition, so the deferred Right assignment happens
            // here, before IsValid is computed.
            if (rightPending)
            {
                expr.Right = new Expression(numericStr);
                rightPending = false;
            }

            isValid = expr.IsValid;

            // [H-4 fix] A trailing partial number ("-", "-.", "+") never fails the FSA -
            // it just sits in numericStr and used to reach CalcValue as a raw string,
            // throwing FormatException instead of ParserException at eval time.
            if (!string.IsNullOrEmpty(numericStr)
                && !double.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                throw new ParserException($"Invalid number '{numericStr}'", updateIndex, expressionStr);
            }

            // Check if all brackets have been properly closed
            if (bracketStack.Count > 0)
            {
                char unmatchedBracket = bracketStack.Peek();
                throw new ParserException($"Unmatched left bracket '{unmatchedBracket}' - expression has {bracketStack.Count} unclosed bracket(s)", updateIndex, expressionStr);
            }
            
            return (isValid, ExpressionParserState.eEnd, updatedExpr, updateIndex, numericStr, functionName);
        }

        /// <summary>
        /// [F-2 fix] True when the node is the untouched root placeholder created by
        /// ParseToExpr (default state, no operator, no children, no bracket, no parent).
        /// Such a node can be discarded when a completed function call should become
        /// the active expression itself.
        /// </summary>
        private static bool IsFreshPlaceholder(Expression expr)
        {
            return expr.Parent == null
                && expr.BracketState == BracketState.eNone
                && expr.State == ExpressionState.eValueOnly
                && expr.Operator == null
                && expr.Left == null
                && expr.Right == null;
        }

        /// <summary>
        /// [F-2/F-3 fix] Attach a completed operand node (function call / negate
        /// wrapper) to the current tree position and return the node that becomes the
        /// active expression:
        ///   - right-operand position (eExpectRightExpression) -> expr.Right
        ///   - untouched root placeholder -> the node itself replaces the placeholder
        ///   - otherwise (bracket holder etc.) -> Left link (valueless, operatorless
        ///     holders collapse transparently in Expression.CalcValue)
        /// </summary>
        private static Expression AttachOperandNode(Expression expr, Expression node, bool rightOperandPosition)
        {
            if (rightOperandPosition)
            {
                expr.Right = node;
            }
            else if (IsFreshPlaceholder(expr))
            {
                return node;
            }
            else
            {
                expr.Left = node;
            }
            return node;
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