using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    public class Util
    {
        static char[] OperatorsArray = new char[] { '+', '-', '*', '/' };
        static char[] PointArray = new char[] { '.' };
        static char[] SignArray = new char[] { '+', '-' };
        static char[] LeftBracketArray = new char[] { '(', '[', '{' };
        static char[] RightBracketArray = new char[] { ')', ']', '}' };
        public static bool IsNumeric(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        public static bool IsSign(char ch)
        {
            return SignArray.Contains(ch);
        }

        public static bool IsPoint(char ch)
        {
            return PointArray.Contains(ch);
        }

        public static bool IsSignNumeric(char ch)
        {
            return IsNumeric(ch) || IsSign(ch);
        }

        // allow +, -, .
        public static bool IsGeneralNumeric(char ch)
        {
            return IsSignNumeric(ch) || PointArray.Contains(ch);
        }

        public static bool IsOperator(char ch)
        {
            return OperatorsArray.Contains(ch);
        }

        public static bool IsLeftBracket(char ch)
        {
            return LeftBracketArray.Contains(ch);
        }

        public static bool IsRightBracket(char ch)
        {
            return RightBracketArray.Contains(ch);
        }

        /// <summary>
        /// Check if character is alphabetic (for function names)
        /// </summary>
        public static bool IsAlpha(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }

        /// <summary>
        /// Check if character could be part of a function name (alphanumeric)
        /// </summary>
        public static bool IsFunctionNameChar(char ch)
        {
            return IsAlpha(ch) || IsNumeric(ch);
        }

        /// <summary>
        /// Check if character is a comma (argument separator for multi-arg functions)
        /// </summary>
        public static bool IsComma(char ch)
        {
            return ch == ',';
        }

        /// <summary>
        /// Get the matching right bracket for a left bracket
        /// </summary>
        /// <param name="leftBracket">Left bracket character</param>
        /// <returns>Matching right bracket character, or '\0' if not a valid left bracket</returns>
        public static char GetMatchingRightBracket(char leftBracket)
        {
            switch (leftBracket)
            {
                case '(':
                    return ')';
                case '[':
                    return ']';
                case '{':
                    return '}';
                default:
                    return '\0';
            }
        }

        /// <summary>
        /// Check if two brackets match (left and right pair)
        /// </summary>
        /// <param name="leftBracket">Left bracket character</param>
        /// <param name="rightBracket">Right bracket character</param>
        /// <returns>True if brackets match correctly</returns>
        public static bool BracketsMatch(char leftBracket, char rightBracket)
        {
            return GetMatchingRightBracket(leftBracket) == rightBracket;
        }
    }
}
