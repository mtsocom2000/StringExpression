using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    /// <summary>
    /// Exception thrown when parsing fails, with detailed context information
    /// </summary>
    public class ParserException : Exception
    {
        /// <summary>
        /// Position in the expression string where the error occurred (0-based)
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// The character or token that caused the error
        /// </summary>
        public string ErrorToken { get; private set; }

        /// <summary>
        /// The full expression being parsed
        /// </summary>
        public string Expression { get; private set; }

        /// <summary>
        /// Context around the error position (substring showing nearby characters)
        /// </summary>
        public string Context { get; private set; }

        /// <summary>
        /// Basic constructor with just a message
        /// </summary>
        public ParserException(string msg) : base(msg)
        {
            Position = -1;
            ErrorToken = null;
            Expression = null;
            Context = null;
        }

        /// <summary>
        /// Constructor with position and expression context
        /// </summary>
        public ParserException(string msg, int position, string expression) : base(FormatMessage(msg, position, expression, null))
        {
            Position = position;
            Expression = expression;
            ErrorToken = GetErrorToken(position, expression);
            Context = GetContext(position, expression);
        }

        /// <summary>
        /// Constructor with position, expression, and error token
        /// </summary>
        public ParserException(string msg, int position, string expression, string errorToken) : base(FormatMessage(msg, position, expression, errorToken))
        {
            Position = position;
            Expression = expression;
            ErrorToken = errorToken ?? GetErrorToken(position, expression);
            Context = GetContext(position, expression);
        }

        /// <summary>
        /// Format the error message with position and context
        /// </summary>
        private static string FormatMessage(string msg, int position, string expression, string errorToken)
        {
            if (position < 0 || expression == null)
            {
                return msg;
            }

            var token = errorToken ?? GetErrorToken(position, expression);
            var context = GetContext(position, expression);

            return $"{msg} at position {position}" +
                   (token != null ? $" (token: '{token}')" : "") +
                   (context != null ? $"\n  Context: ...{context}..." : "");
        }

        /// <summary>
        /// Get the error token at the given position
        /// </summary>
        private static string GetErrorToken(int position, string expression)
        {
            if (position < 0 || position >= expression.Length)
            {
                return null;
            }
            return expression[position].ToString();
        }

        /// <summary>
        /// Get context around the error position (10 chars before and after)
        /// </summary>
        private static string GetContext(int position, string expression)
        {
            if (expression == null || position < 0)
            {
                return null;
            }

            int start = Math.Max(0, position - 10);
            int end = Math.Min(expression.Length, position + 11);

            if (start == 0 && end == expression.Length)
            {
                return expression;
            }

            return expression.Substring(start, end - start);
        }

        /// <summary>
        /// Create a detailed error string with visual pointer to error position
        /// </summary>
        public override string ToString()
        {
            var result = base.ToString();

            if (Position >= 0 && Expression != null)
            {
                result += $"\n\nExpression: {Expression}";
                result += $"\nPosition:   {Position}";

                // Visual error pointer
                if (Position < Expression.Length)
                {
                    var pointer = new StringBuilder();
                    for (int i = 0; i < Position; i++)
                    {
                        pointer.Append(' ');
                    }
                    pointer.Append('^');
                    result += $"\n            {pointer}";
                }
            }

            return result;
        }
    }
}