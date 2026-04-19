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
            _supportOperators.Add(new OperatorTan());      // NEW
            _supportOperators.Add(new OperatorAsin());     // NEW
            _supportOperators.Add(new OperatorAcos());     // NEW
            _supportOperators.Add(new OperatorAtan());     // NEW
            _supportOperators.Add(new OperatorSqrt());
            // Logarithmic functions
            _supportOperators.Add(new OperatorLog());      // Natural log (ln)
            _supportOperators.Add(new OperatorLog10());    // NEW - Common log (base 10)
            _supportOperators.Add(new OperatorLn());       // NEW - Alias for natural log
            // Other functions
            _supportOperators.Add(new OperatorAbs());
            _supportOperators.Add(new OperatorPow());
            _supportOperators.Add(new OperatorExp());
        }
        private static object _locker = new object();
        private static OperatorFactory instance = null;
        private static List<Operator> _supportOperators;
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
