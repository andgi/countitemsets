using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CountItemSets
{
    public delegate void GenerateCallBack();

    public interface IFrequentItemsetGenerator
    {
        IDictionary<long, int> Level1 { get; }
        IDictionary<string, int> Level2 { get; }
        IDictionary<string, int> Level3 { get; }
        IDictionary<string, int> Level4 { get; }
        IDictionary<string, int> Level5 { get; }

        void BeginGenerate(string fileNameTransaction, GenerateCallBack callBack);
        int GetTransactionCount();
        void SetPruningMinSupport(double minSupport);

        int GetProgess();
        Stopwatch GetStopWatch();
        Dictionary<long, int> GetDictionaryEANtoVGR();

    }
}
