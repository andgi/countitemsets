using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountItemSets
{
    class TransactionContext
    {
        static Dictionary<long, string> dictionaryEAN = new Dictionary<long, string>();
        static Dictionary<int, string> dictionaryVGR = new Dictionary<int, string>();
        static Dictionary<long, int> dictionaryEANtoVGR = new Dictionary<long, int>();
        static Dictionary<string, double> dictionaryRule = new Dictionary<string, double>();
    }
}
