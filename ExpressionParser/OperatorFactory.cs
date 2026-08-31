using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    public class OperatorFactory
    {
        private OperatorFactory()
        {
            _supportOperators = new List<Operator>();
            // Basic operators
            _supportOperators.Add(new OperatorAdd());
            _supportOperators.Add(new OperatorSub());
            _supportOperators.Add(new OperatorMultiple());
            _supportOperators.Add(new OperatorDivide());
            // Function operators
            // Math functions
            _supportOperators.Add(new OperatorSin());
            _supportOperators.Add(new OperatorCos());
            _supportOperators.Add(new OperatorTan());
            _supportOperators.Add(new OperatorAsin());
            _supportOperators.Add(new OperatorAcos());
            _supportOperators.Add(new OperatorAtan());
            _supportOperators.Add(new OperatorSqrt());
            // Logarithmic functions
            _supportOperators.Add(new OperatorLog());      // Natural log
            _supportOperators.Add(new OperatorLog10());    // Common log (base 10)
            _supportOperators.Add(new OperatorLn());       // Alias for natural log
            // Other functions
            _supportOperators.Add(new OperatorAbs());
            _supportOperators.Add(new OperatorPow());
            _supportOperators.Add(new OperatorExp());

            // Symbol lookup map (P-4): O(1) lookup replaces the linear
            // Support() scan for the common path. OrdinalIgnoreCase keeps
            // function-name matching culture- and case-insensitive.
            _symbolMap = new Dictionary<string, Operator>(_supportOperators.Count * 2, StringComparer.OrdinalIgnoreCase);
            foreach (var op in _supportOperators)
            {
                if (!string.IsNullOrEmpty(op.Symbol))
                {
                    _symbolMap[op.Symbol] = op;
                }
            }
        }
        private static object _locker = new object();
        private static OperatorFactory instance = null;
        private static List<Operator> _supportOperators;
        private static Dictionary<string, Operator> _symbolMap;
        public static OperatorFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_locker)
                    {
                        if (instance == null)
                            instance = new OperatorFactory();
                    }
                }
                return instance;
            }
        }

        public Operator Support(string opStr)
        {
            // Fast path: canonical symbol lookup (covers +,-,*,/ and all
            // function names via FunctionOperator.Symbol => Name).
            if (!string.IsNullOrEmpty(opStr) && _symbolMap.TryGetValue(opStr, out var mapped))
            {
                return mapped;
            }

            // Fallback: keep the linear Support() scan for any operator that
            // matches non-canonically (future aliases, custom operators).
            foreach (var op in _supportOperators)
            {
                if (op.Support(opStr))
                {
                    return op;
                }
            }
            return null;
        }
    }
}
