using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace CountItemSets
{
    public class ParallelTransactionFrequentItemsetGenerator : IFrequentItemsetGenerator
    {
        public IDictionary<long, int> Level1 { get { return dictionaryLevel1; } }
        public IDictionary<string, int> Level2 { get { return dictionaryLevel2; } }
        public IDictionary<string, int> Level3 { get { return dictionaryLevel3; } }
        public IDictionary<string, int> Level4 { get { return dictionaryLevel4; } }
        public IDictionary<string, int> Level5 { get { return dictionaryLevel5; } }

        private ConcurrentDictionary<long, int> dictionaryLevel1 = new ConcurrentDictionary<long, int>();
        private ConcurrentDictionary<string, int> dictionaryLevel2 = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, int> dictionaryLevel3 = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, int> dictionaryLevel4 = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, int> dictionaryLevel5 = new ConcurrentDictionary<string, int>();

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
            List<TransactionReader.Transaction> transactions;
            while ((transactions = reader.ReadList(1000)) != null)
            {
                transactionCount += transactions.Count; 
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        //List<long> vgrs = transactions[t].VGRCodes;
                        foreach (long eanNr in keys)
                        {
                            dictionaryLevel1.AddOrUpdate(eanNr, 1, (key, value) => value + 1);
                        }
                        /*foreach (long vgrNr in vgrs)
                        {
                            dictionaryLevel1.AddOrUpdate(vgrNr, 1, (key, value) => value + 1);
                        }*/
                    }
                });
            }
            Console.Out.WriteLine("Found " + dictionaryLevel1.Count + " 1-tuples.");
            dictionaryLevel1 = new ConcurrentDictionary<long,int>(dictionaryLevel1.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport && !pruningExcludeItems.Contains(item.Key)));
            Console.Out.WriteLine("Found " + dictionaryLevel1.Count + " interesting 1-tuples.");
            progressGenerate = 10;

            // Level 2
            dictionaryLevel2.Clear();
            // E1 E2
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                            dictionaryLevel2.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                        }
                                    }
                            }
                    }
                });
            }
            // E1 V1
            /*reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                            dictionaryLevel2.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                        }
                                    }
                            }
                    }
                });
            }
            // V1 V2
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                            dictionaryLevel2.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                        }
                                    }
                            }
                    }
                });
            }*/
            Console.Out.WriteLine("Found " + dictionaryLevel2.Count + " 2-tuples.");
            dictionaryLevel2 = new ConcurrentDictionary<string,int>(dictionaryLevel2.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport));
            Console.Out.WriteLine("Found " + dictionaryLevel2.Count + " interesting 2-tuples.");
            progressGenerate = 40;

            // Level 3
            dictionaryLevel3.Clear();
            // E1 E2 E3
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                                    dictionaryLevel3.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }
            /*
            // E1 E2 V1
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                                    // && dictionaryEANtoVGR[key1] != -key3 && dictionaryEANtoVGR[key2] != -key3
                                                    )
                                                {
                                                    string keyName = key1 + "," + key2 + "," + key3;
                                                    dictionaryLevel3.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }
            // E1 V1 V2
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                                    // && dictionaryEANtoVGR[key1] != -key2 && dictionaryEANtoVGR[key1] != -key3
                                                    )
                                                {
                                                    string keyName = key1 + "," + key2 + "," + key3;
                                                    dictionaryLevel3.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }

            // V1 V2 V3                        
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].VGRCodes;
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
                                                    dictionaryLevel3.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }
            */
            Console.Out.WriteLine("Found " + dictionaryLevel3.Count + " 3-tuples.");
            dictionaryLevel3 = new ConcurrentDictionary<string,int>(dictionaryLevel3.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport));
            Console.Out.WriteLine("Found " + dictionaryLevel3.Count + " interesting 3-tuples.");
            progressGenerate = 60;

            // Level 4
            dictionaryLevel4.Clear();
            // E1 E2 E3 E4
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                                            dictionaryLevel4.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                        }
                                                    }
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }
            /*
            // E1 E2 E3 V1
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                                            //&& dictionaryEANtoVGR[key1] != -key4
                                                            //&& dictionaryEANtoVGR[key2] != -key4
                                                            //&& dictionaryEANtoVGR[key3] != -key4 *)
                                                        {
                                                            string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                            dictionaryLevel4.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                        }
                                                    }
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }

            // E1 E2 V1 V2
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                                            //&& dictionaryEANtoVGR[key1] != -key3
                                                            //&& dictionaryEANtoVGR[key2] != -key3
                                                            //&& dictionaryEANtoVGR[key1] != -key4
                                                            //&& dictionaryEANtoVGR[key2] != -key4
                                                           )
                                                        {
                                                            string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                            dictionaryLevel4.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                        }
                                                    }
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }

            // E1 V1 V2 V3
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                                            //&& dictionaryEANtoVGR[key1] != -key2
                                                            //&& dictionaryEANtoVGR[key1] != -key3
                                                            //&& dictionaryEANtoVGR[key1] != -key4
                                                           )
                                                        {
                                                            string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                            dictionaryLevel4.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                        }
                                                    }
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }

            // V1 V2 V3 V4
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].VGRCodes;
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
                                                            dictionaryLevel4.AddOrUpdate(keyName, 1, (key, value) => value + 1);
                                                        }
                                                    }
                                                }
                                            }
                                    }
                            }
                        }
                    }
                });
            }
            */
            Console.Out.WriteLine("Found " + dictionaryLevel4.Count + " 4-tuples.");
            dictionaryLevel4 = new ConcurrentDictionary<string,int>(dictionaryLevel4.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport));
            Console.Out.WriteLine("Found " + dictionaryLevel4.Count + " interesting 4-tuples.");
            progressGenerate = 80;

            // Level 5
            dictionaryLevel5.Clear();
            // E1 E2 E3 E4 E5
            reader.Begin();
            while ((transactions = reader.ReadList(1000)) != null)
            {
                Parallel.ForEach(Partitioner.Create(0, transactions.Count), range =>
                {
                    for (int t = range.Item1; t < range.Item2; t++)
                    {
                        List<long> keys = transactions[t].EANCodes;
                        List<long> vgrs = transactions[t].VGRCodes;
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
                                                                    dictionaryLevel5.AddOrUpdate(keyName, 1, (key, value) => value + 1);
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
                });
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
            Console.Out.WriteLine("Found " + dictionaryLevel5.Count + " 5-tuples.");
            dictionaryLevel5 = new ConcurrentDictionary<string,int>(dictionaryLevel5.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport));
            Console.Out.WriteLine("Found " + dictionaryLevel5.Count + " interesting 5-tuples.");

            stopwatch.Stop();
            //textBoxTime.Text = stopwatch.Elapsed.ToString();
            progressGenerate = 100;

            if(callBack != null) callBack();
        }
    }
}
