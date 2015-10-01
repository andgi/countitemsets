using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace CountItemSets
{
    public class SequentialFrequentItemsetGenerator : IFrequentItemsetGenerator
    {
        public IDictionary<long, int> Level1 { get { return dictionaryLevel1; } }
        public IDictionary<string, int> Level2 { get { return dictionaryLevel2; } }
        public IDictionary<string, int> Level3 { get { return dictionaryLevel3; } }
        public IDictionary<string, int> Level4 { get { return dictionaryLevel4; } }
        public IDictionary<string, int> Level5 { get { return dictionaryLevel5; } }

        private Dictionary<long, int> dictionaryLevel1 = new Dictionary<long, int>();
        private Dictionary<string, int> dictionaryLevel2 = new Dictionary<string, int>();
        private Dictionary<string, int> dictionaryLevel3 = new Dictionary<string, int>();
        private Dictionary<string, int> dictionaryLevel4 = new Dictionary<string, int>();
        private Dictionary<string, int> dictionaryLevel5 = new Dictionary<string, int>();

        HashSet<long> pruningExcludeItems = new HashSet<long>();

        private string fileNameTransaction; // Should be handled by separate class TransactionReader
        private int transactionCount; // Should be handled by separate class TransactionReader
        private Dictionary<long, int> dictionaryEANtoVGR = new Dictionary<long, int>(); // Should be handled by separate class TransactionContext
        private string fileNameExcludeItems; // Should be handled by separate class

        private int progressGenerate = 0;
        private Stopwatch stopwatch = new Stopwatch();
        private double pruningMinSupport = 0.0001;
        private int maxNrTransactions = 1000000000;
        public void SetMaxNrTransactions(int maxNrTransactions)
        {
            this.maxNrTransactions = maxNrTransactions;
        }

        public void SetPruningMinSupport(double minSupport)
        {
            pruningMinSupport = minSupport;
        }

        public int GetTransactionCount()
        {
            return transactionCount;
        }

        public int GetProgess()
        {
            return progressGenerate;
        }

        public Stopwatch GetStopWatch()
        {
            return stopwatch;
        }

        public Dictionary<long, int> GetDictionaryEANtoVGR()
        {
            return dictionaryEANtoVGR;
        }

        public void BeginGenerate(string fileNameTransaction, GenerateCallBack callBack)
        {
            this.fileNameTransaction = fileNameTransaction;
            Thread thread = new Thread(new ParameterizedThreadStart(GenerateThread));
            thread.Name = "GenaratorThread";
            thread.Start(callBack);
        }
        public void Generate(string fileNameTransaction)
        {
            this.fileNameTransaction = fileNameTransaction;
            GenerateThread(null);
        }

        private void GenerateThread(object obj)
        {
            GenerateCallBack callBack = obj as GenerateCallBack;
            progressGenerate = 0;
            stopwatch.Restart();

            TransactionReader reader = new TransactionReader(fileNameTransaction, dictionaryEANtoVGR);
            reader.SetMaxNrTransactions(maxNrTransactions);

            // Level 1
            dictionaryLevel1.Clear();
            // E1
            // V1
            reader.Begin();
            transactionCount = 0;
            while (reader.Read())
            {
                transactionCount++;
                List<long> keys = reader.Current.EANCodes;
                List<long> vgrs = reader.Current.VGRCodes;
                foreach (long eanNr in keys)
                {
                    if (dictionaryLevel1.ContainsKey(eanNr))
                        dictionaryLevel1[eanNr]++;
                    else
                        dictionaryLevel1.Add(eanNr, 1);
                }
                foreach (long vgrNr in vgrs)
                {
                    if (dictionaryLevel1.ContainsKey(vgrNr))
                        dictionaryLevel1[vgrNr]++;
                    else
                        dictionaryLevel1.Add(vgrNr, 1);
                }
            }
            dictionaryLevel1 = dictionaryLevel1.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport && !pruningExcludeItems.Contains(item.Key)).ToDictionary(item => item.Key, item => item.Value);
            progressGenerate = 10;

            // Level 2
            dictionaryLevel2.Clear();
            // E1 E2
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                if (keys.Count > 1)
                    for (int i = 0; i < (keys.Count - 1); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < keys.Count; j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel1.ContainsKey(key2))
                                {
                                    string keyName = key1 + "," + key2;
                                    if (dictionaryLevel2.ContainsKey(keyName))
                                        dictionaryLevel2[keyName]++;
                                    else
                                        dictionaryLevel2.Add(keyName, 1);
                                }
                            }
                    }
            }
            // E1 V1
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                List<long> vgrs = reader.Current.VGRCodes;
                if (vgrs.Count > 0 && keys.Count > 0)
                    for (int i = 0; i < keys.Count; i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = 0; j < vgrs.Count; j++)
                            {
                                long key2 = vgrs[j];
                                if (dictionaryLevel1.ContainsKey(key2) && dictionaryEANtoVGR[key1] != -key2)
                                {
                                    string keyName = key1 + "," + key2;
                                    if (dictionaryLevel2.ContainsKey(keyName))
                                        dictionaryLevel2[keyName]++;
                                    else
                                        dictionaryLevel2.Add(keyName, 1);
                                }
                            }
                    }
            }
            // V1 V2
            reader.Begin();
            while (reader.Read())
            {
                List<long> vgrs = reader.Current.VGRCodes;
                if (vgrs.Count > 1)
                    for (int i = 0; i < (vgrs.Count - 1); i++)
                    {
                        long key1 = vgrs[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < vgrs.Count; j++)
                            {
                                long key2 = vgrs[j];
                                if (dictionaryLevel1.ContainsKey(key2))
                                {
                                    string keyName = key1 + "," + key2;
                                    if (dictionaryLevel2.ContainsKey(keyName))
                                        dictionaryLevel2[keyName]++;
                                    else
                                        dictionaryLevel2.Add(keyName, 1);
                                }
                            }
                    }
            }
            dictionaryLevel2 = dictionaryLevel2.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport).ToDictionary(item => item.Key, item => item.Value);
            progressGenerate = 40;

            // Level 3
            dictionaryLevel3.Clear();
            // E1 E2 E3
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                if (keys.Count > 2)
                {
                    for (int i = 0; i < (keys.Count - 2); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 1); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < keys.Count; k++)
                                    {
                                        long key3 = keys[k];
                                        if (dictionaryLevel2.ContainsKey(key2 + "," + key3) && dictionaryLevel2.ContainsKey(key1 + "," + key3))
                                        {
                                            string keyName = key1 + "," + key2 + "," + key3;
                                            if (dictionaryLevel3.ContainsKey(keyName))
                                                dictionaryLevel3[keyName]++;
                                            else
                                                dictionaryLevel3.Add(keyName, 1);
                                        }
                                    }
                            }
                    }
                }
            }
            // E1 E2 V1
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                List<long> vgrs = reader.Current.VGRCodes;
                if (keys.Count > 1 && vgrs.Count > 0)
                {
                    for (int i = 0; i < (keys.Count - 1); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 0); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = 0; k < vgrs.Count; k++)
                                    {
                                        long key3 = vgrs[k];
                                        if (dictionaryLevel2.ContainsKey(key2 + "," + key3) && dictionaryLevel2.ContainsKey(key1 + "," + key3)
                                            /* && dictionaryEANtoVGR[key1] != -key3 && dictionaryEANtoVGR[key2] != -key3 */)
                                        {
                                            string keyName = key1 + "," + key2 + "," + key3;
                                            if (dictionaryLevel3.ContainsKey(keyName))
                                                dictionaryLevel3[keyName]++;
                                            else
                                                dictionaryLevel3.Add(keyName, 1);
                                        }
                                    }
                            }
                    }
                }
            }
            // E1 V1 V2
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                List<long> vgrs = reader.Current.VGRCodes;
                if (keys.Count > 0 && vgrs.Count > 1)
                {
                    for (int i = 0; i < (keys.Count); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = 0; j < (vgrs.Count - 1); j++)
                            {
                                long key2 = vgrs[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < vgrs.Count; k++)
                                    {
                                        long key3 = vgrs[k];
                                        if (dictionaryLevel2.ContainsKey(key2 + "," + key3) && dictionaryLevel2.ContainsKey(key1 + "," + key3)
                                            /* && dictionaryEANtoVGR[key1] != -key2 && dictionaryEANtoVGR[key1] != -key3 */)
                                        {
                                            string keyName = key1 + "," + key2 + "," + key3;
                                            if (dictionaryLevel3.ContainsKey(keyName))
                                                dictionaryLevel3[keyName]++;
                                            else
                                                dictionaryLevel3.Add(keyName, 1);
                                        }
                                    }
                            }
                    }
                }
            }

            // V1 V2 V3                        
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.VGRCodes;
                if (keys.Count > 2)
                {
                    for (int i = 0; i < (keys.Count - 2); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 1); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < keys.Count; k++)
                                    {
                                        long key3 = keys[k];
                                        if (dictionaryLevel2.ContainsKey(key2 + "," + key3) && dictionaryLevel2.ContainsKey(key1 + "," + key3))
                                        {
                                            string keyName = key1 + "," + key2 + "," + key3;
                                            if (dictionaryLevel3.ContainsKey(keyName))
                                                dictionaryLevel3[keyName]++;
                                            else
                                                dictionaryLevel3.Add(keyName, 1);
                                        }
                                    }
                            }
                    }
                }
            }
            dictionaryLevel3 = dictionaryLevel3.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport).ToDictionary(item => item.Key, item => item.Value);
            progressGenerate = 60;

            // Level 4
            dictionaryLevel4.Clear();
            // E1 E2 E3 E4
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                if (keys.Count > 3)
                {
                    for (int i = 0; i < (keys.Count - 3); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 2); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < (keys.Count - 1); k++)
                                    {
                                        long key3 = keys[k];
                                        if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3))
                                        {
                                            for (int l = k + 1; l < keys.Count; l++)
                                            {
                                                long key4 = keys[l];
                                                if (dictionaryLevel3.ContainsKey(key2 + "," + key3 + "," + key4) && dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key4) && dictionaryLevel3.ContainsKey(key1 + "," + key3 + "," + key4))
                                                {
                                                    string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                    if (dictionaryLevel4.ContainsKey(keyName))
                                                        dictionaryLevel4[keyName]++;
                                                    else
                                                        dictionaryLevel4.Add(keyName, 1);
                                                }
                                            }
                                        }
                                    }
                            }
                    }
                }
            }

            // E1 E2 E3 V1
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                List<long> vgrs = reader.Current.VGRCodes;
                if (keys.Count > 2 && vgrs.Count > 0)
                {
                    for (int i = 0; i < (keys.Count - 2); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 1); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < (keys.Count); k++)
                                    {
                                        long key3 = keys[k];
                                        if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3))
                                        {
                                            for (int l = 0; l < vgrs.Count; l++)
                                            {
                                                long key4 = vgrs[l];
                                                if (dictionaryLevel3.ContainsKey(key2 + "," + key3 + "," + key4)
                                                    && dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key4)
                                                    && dictionaryLevel3.ContainsKey(key1 + "," + key3 + "," + key4)
                                                    /* && dictionaryEANtoVGR[key1] != -key4
                                                    && dictionaryEANtoVGR[key2] != -key4
                                                    && dictionaryEANtoVGR[key3] != -key4 */)
                                                {
                                                    string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                    if (dictionaryLevel4.ContainsKey(keyName))
                                                        dictionaryLevel4[keyName]++;
                                                    else
                                                        dictionaryLevel4.Add(keyName, 1);
                                                }
                                            }
                                        }
                                    }
                            }
                    }
                }
            }

            // E1 E2 V1 V2
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                List<long> vgrs = reader.Current.VGRCodes;
                if (keys.Count > 1 && vgrs.Count > 1)
                {
                    for (int i = 0; i < (keys.Count - 1); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 0); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = 0; k < (vgrs.Count - 1); k++)
                                    {
                                        long key3 = vgrs[k];
                                        if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3))
                                        {
                                            for (int l = k + 1; l < vgrs.Count; l++)
                                            {
                                                long key4 = vgrs[l];
                                                if (dictionaryLevel3.ContainsKey(key2 + "," + key3 + "," + key4)
                                                    && dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key4)
                                                    && dictionaryLevel3.ContainsKey(key1 + "," + key3 + "," + key4)
                                                    /* && dictionaryEANtoVGR[key1] != -key3
                                                    && dictionaryEANtoVGR[key2] != -key3
                                                    && dictionaryEANtoVGR[key1] != -key4
                                                    && dictionaryEANtoVGR[key2] != -key4 */)
                                                {
                                                    string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                    if (dictionaryLevel4.ContainsKey(keyName))
                                                        dictionaryLevel4[keyName]++;
                                                    else
                                                        dictionaryLevel4.Add(keyName, 1);
                                                }
                                            }
                                        }
                                    }
                            }
                    }
                }
            }

            // E1 V1 V2 V3
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                List<long> vgrs = reader.Current.VGRCodes;
                if (keys.Count > 0 && vgrs.Count > 2)
                {
                    for (int i = 0; i < (keys.Count - 0); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = 0; j < (vgrs.Count - 2); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < (vgrs.Count - 1); k++)
                                    {
                                        long key3 = vgrs[k];
                                        if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3))
                                        {
                                            for (int l = k + 1; l < vgrs.Count; l++)
                                            {
                                                long key4 = vgrs[l];
                                                if (dictionaryLevel3.ContainsKey(key2 + "," + key3 + "," + key4)
                                                    && dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key4)
                                                    && dictionaryLevel3.ContainsKey(key1 + "," + key3 + "," + key4)
                                                    /* && dictionaryEANtoVGR[key1] != -key2
                                                    && dictionaryEANtoVGR[key1] != -key3
                                                    && dictionaryEANtoVGR[key1] != -key4 */)
                                                {
                                                    string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                    if (dictionaryLevel4.ContainsKey(keyName))
                                                        dictionaryLevel4[keyName]++;
                                                    else
                                                        dictionaryLevel4.Add(keyName, 1);
                                                }
                                            }
                                        }
                                    }
                            }
                    }
                }
            }

            // V1 V2 V3 V4
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.VGRCodes;
                if (keys.Count > 3)
                {
                    for (int i = 0; i < (keys.Count - 3); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 2); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < (keys.Count - 1); k++)
                                    {
                                        long key3 = keys[k];
                                        if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3))
                                        {
                                            for (int l = k + 1; l < keys.Count; l++)
                                            {
                                                long key4 = keys[l];
                                                if (dictionaryLevel3.ContainsKey(key2 + "," + key3 + "," + key4) && dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key4) && dictionaryLevel3.ContainsKey(key1 + "," + key3 + "," + key4))
                                                {
                                                    string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                    if (dictionaryLevel4.ContainsKey(keyName))
                                                        dictionaryLevel4[keyName]++;
                                                    else
                                                        dictionaryLevel4.Add(keyName, 1);
                                                }
                                            }
                                        }
                                    }
                            }
                    }
                }
            }
            dictionaryLevel4 = dictionaryLevel4.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport).ToDictionary(item => item.Key, item => item.Value);
            progressGenerate = 80;

            // Level 5
            dictionaryLevel5.Clear();
            // E1 E2 E3 E4 E5
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.EANCodes;
                List<long> vgrs = reader.Current.VGRCodes;
                if (keys.Count > 4)
                {
                    for (int i = 0; i < (keys.Count - 4); i++)
                    {
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 3); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < (keys.Count - 2); k++)
                                    {
                                        long key3 = keys[k];
                                        if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3))
                                        {
                                            for (int l = k + 1; l < (keys.Count - 1); l++)
                                            {
                                                long key4 = keys[l];
                                                if (dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key3 + "," + key4))
                                                {
                                                    for (int m = l + 1; m < (keys.Count); m++)
                                                    {
                                                        long key5 = keys[m];
                                                        if (dictionaryLevel4.ContainsKey(key2 + "," + key3 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key3 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key3 + "," + key5))
                                                        {
                                                            string keyName = key1 + "," + key2 + "," + key3 + "," + key4 + "," + key5;
                                                            if (dictionaryLevel5.ContainsKey(keyName))
                                                                dictionaryLevel5[keyName]++;
                                                            else
                                                                dictionaryLevel5.Add(keyName, 1);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                            }
                    }
                }
            }

            // V1 V2 V3 V4 V5
            /*            
            reader.Begin();
            while (reader.Read())
            {
                List<long> keys = reader.Current.VGRCodes;
                if (keys.Count > 4)
                {
                    List<string>[] keyNames = new List<string>[keys.Count - 4];
                    Parallel.For(0, keys.Count - 4, i =>
                    {
                        keyNames[i] = new List<string>();
                        long key1 = keys[i];
                        if (dictionaryLevel1.ContainsKey(key1))
                            for (int j = i + 1; j < (keys.Count - 3); j++)
                            {
                                long key2 = keys[j];
                                if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                    for (int k = j + 1; k < (keys.Count - 2); k++)
                                    {
                                        long key3 = keys[k];
                                        if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3))
                                        {
                                            for (int l = j + 1; l < (keys.Count - 1); l++)
                                            {
                                                long key4 = keys[l];
                                                if (dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key3 + "," + key4))
                                                {
                                                    for (int m = l + 1; m < (keys.Count); m++)
                                                    {
                                                        long key5 = keys[m];
                                                        if (dictionaryLevel4.ContainsKey(key2 + "," + key3 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key3 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key3 + "," + key5))
                                                        {
                                                            string keyName = key1 + "," + key2 + "," + key3 + "," + key4 + "," + key5;
                                                            keyNames[i].Add(keyName);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                            }
                    }); //Parallel.For
                    for (int i = 0; i < (keys.Count - 4); i++)
                    {
                        foreach (string keyName in keyNames[i])
                        {
                            if (dictionaryLevel5.ContainsKey(keyName))
                                dictionaryLevel5[keyName]++;
                            else
                                dictionaryLevel5.Add(keyName, 1);
                        }
                    }
                }
            }
            */
            dictionaryLevel5 = dictionaryLevel5.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport).ToDictionary(item => item.Key, item => item.Value);

            stopwatch.Stop();
            //textBoxTime.Text = stopwatch.Elapsed.ToString();
            progressGenerate = 100;

            if(callBack != null) callBack();
        }
    }
}
