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
        static char[] RightBracketArray = new char[] { ')', ']', '|' };
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
    }
}
