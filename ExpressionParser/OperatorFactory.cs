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
            _supportOperators.Add(new OperatorAdd());
            _supportOperators.Add(new OperatorSub());
            _supportOperators.Add(new OperatorMultiple());
            _supportOperators.Add(new OperatorDivide());
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
