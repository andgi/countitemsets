using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace CountItemSets
{
    public class ParallelFrequentItemsetGenerator : IFrequentItemsetGenerator
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

        private void GenerateThread(object obj)
        {
            GenerateCallBack callBack = obj as GenerateCallBack;
            progressGenerate = 0;
            stopwatch.Start();
            StreamReader reader = new StreamReader(fileNameTransaction);
            int rowCount = 0;
            int transNrLast = 0;
            List<long> keys = new List<long>(10);
            List<long> vgrs = new List<long>(10);
            dictionaryLevel1.Clear();
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                            transactionCount++;
                        }
                        if (transNrLast != transNr)
                        {
                            vgrs.Sort();
                            vgrs = new List<long>(vgrs.Distinct());
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
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
                            keys.Clear();
                            vgrs.Clear();
                            transNrLast = transNr;
                            transactionCount++;
                        }
                        {
                            long eanNr = Int64.Parse(columns[1]);
                            int vgrNr = Int32.Parse(columns[2]);
                            if (!dictionaryEANtoVGR.ContainsKey(eanNr))
                                dictionaryEANtoVGR.Add(eanNr, vgrNr);
                            vgrs.Add(-vgrNr);
                            keys.Add(eanNr);
                        }
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            dictionaryLevel1 = dictionaryLevel1.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport && !pruningExcludeItems.Contains(item.Key)).ToDictionary(item => item.Key, item => item.Value);

            progressGenerate = 10;
            dictionaryLevel2.Clear();
            // E1 E2
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
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
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // E1 V1
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            vgrs = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            vgrs.Sort();
                            vgrs = new List<long>(vgrs.Distinct());
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
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
                            vgrs.Clear();
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        vgrs.Add(-vgrNr);
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // V1 V2
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            vgrs = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            vgrs.Sort();
                            vgrs = new List<long>(vgrs.Distinct());
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
                            vgrs.Clear();
                            transNrLast = transNr;
                        }
                        vgrs.Add(-vgrNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            dictionaryLevel2 = dictionaryLevel2.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport).ToDictionary(item => item.Key, item => item.Value);

            progressGenerate = 40;
            // E1 E2 E3
            dictionaryLevel3.Clear();
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 2)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 2];
                                Parallel.For(0, keys.Count - 2, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                        keyNames[i].Add(keyName);
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 2); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel3.ContainsKey(keyName))
                                            dictionaryLevel3[keyName]++;
                                        else
                                            dictionaryLevel3.Add(keyName, 1);
                                    }
                                }
                            }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // E1 E2 V1
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            vgrs = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            vgrs.Sort();
                            vgrs = new List<long>(vgrs.Distinct());
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 1 && vgrs.Count > 0)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 1];
                                Parallel.For(0, keys.Count - 1, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                        keyNames[i].Add(keyName);
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 1); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel3.ContainsKey(keyName))
                                            dictionaryLevel3[keyName]++;
                                        else
                                            dictionaryLevel3.Add(keyName, 1);
                                    }
                                }
                            }
                            vgrs.Clear();
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        vgrs.Add(-vgrNr);
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // E1 V1 V2
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            vgrs = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            vgrs.Sort();
                            vgrs = new List<long>(vgrs.Distinct());
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 0 && vgrs.Count > 1)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count];
                                Parallel.For(0, keys.Count, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                        keyNames[i].Add(keyName);
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel3.ContainsKey(keyName))
                                            dictionaryLevel3[keyName]++;
                                        else
                                            dictionaryLevel3.Add(keyName, 1);
                                    }
                                }
                            }
                            vgrs.Clear();
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        vgrs.Add(-vgrNr);
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // V1 V2 V3            
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 2)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 2];
                                Parallel.For(0, keys.Count - 2, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                        keyNames[i].Add(keyName);
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 2); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel3.ContainsKey(keyName))
                                            dictionaryLevel3[keyName]++;
                                        else
                                            dictionaryLevel3.Add(keyName, 1);
                                    }
                                }
                            }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(-vgrNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();

            dictionaryLevel3 = dictionaryLevel3.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport).ToDictionary(item => item.Key, item => item.Value);

            progressGenerate = 60;
            // E1 E2 E3 E4
            dictionaryLevel4.Clear();
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 3)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 3];
                                Parallel.For(0, keys.Count - 3, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                                keyNames[i].Add(keyName);
                                                            }
                                                        }
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 3); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel4.ContainsKey(keyName))
                                            dictionaryLevel4[keyName]++;
                                        else
                                            dictionaryLevel4.Add(keyName, 1);
                                    }
                                }
                            }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // E1 E2 E3 V1
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            vgrs = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            vgrs.Sort();
                            vgrs = new List<long>(vgrs.Distinct());
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 2 && vgrs.Count > 0)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 2];
                                Parallel.For(0, keys.Count - 2, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                                keyNames[i].Add(keyName);
                                                            }
                                                        }
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 2); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel4.ContainsKey(keyName))
                                            dictionaryLevel4[keyName]++;
                                        else
                                            dictionaryLevel4.Add(keyName, 1);
                                    }
                                }
                            }
                            vgrs.Clear();
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        vgrs.Add(-vgrNr);
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // E1 E2 V1 V2
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            vgrs = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            vgrs.Sort();
                            vgrs = new List<long>(vgrs.Distinct());
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 1 && vgrs.Count > 1)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 1];
                                Parallel.For(0, keys.Count - 1, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                                keyNames[i].Add(keyName);
                                                            }
                                                        }
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 1); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel4.ContainsKey(keyName))
                                            dictionaryLevel4[keyName]++;
                                        else
                                            dictionaryLevel4.Add(keyName, 1);
                                    }
                                }
                            }
                            vgrs.Clear();
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        vgrs.Add(-vgrNr);
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // E1 V1 V2 V3
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            vgrs = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            vgrs.Sort();
                            vgrs = new List<long>(vgrs.Distinct());
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 0 && vgrs.Count > 2)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 0];
                                Parallel.For(0, keys.Count - 0, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                                keyNames[i].Add(keyName);
                                                            }
                                                        }
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 0); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel4.ContainsKey(keyName))
                                            dictionaryLevel4[keyName]++;
                                        else
                                            dictionaryLevel4.Add(keyName, 1);
                                    }
                                }
                            }
                            vgrs.Clear();
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        vgrs.Add(-vgrNr);
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // V1 V2 V3 V4
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 3)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 3];
                                Parallel.For(0, keys.Count - 3, i =>
                                {
                                    keyNames[i] = new List<string>();
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
                                                                keyNames[i].Add(keyName);
                                                            }
                                                        }
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 3); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel4.ContainsKey(keyName))
                                            dictionaryLevel4[keyName]++;
                                        else
                                            dictionaryLevel4.Add(keyName, 1);
                                    }
                                }
                            }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(-vgrNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();

            dictionaryLevel4 = dictionaryLevel4.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport).ToDictionary(item => item.Key, item => item.Value);

            progressGenerate = 80;
            // E1 E2 E3 E4 E5
            dictionaryLevel5.Clear();
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
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
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            // V1 V2 V3 V4 V5
            /*
            reader = new StreamReader(fileNameTransaction);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
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
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(-vgrNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            */
            dictionaryLevel5 = dictionaryLevel5.Where(item => ((double)item.Value / transactionCount) >= pruningMinSupport).ToDictionary(item => item.Key, item => item.Value);

            stopwatch.Stop();
            //textBoxTime.Text = stopwatch.Elapsed.ToString();
            progressGenerate = 100;

            callBack();
        }

        private void LoadPruningExcludeItems()
        {
            pruningExcludeItems.Clear();
            if (fileNameExcludeItems == "")
                return;
            StreamReader reader = new StreamReader(fileNameExcludeItems);
            int rowCount = 0;
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        long eanNr = Int64.Parse(columns[0]);
                        pruningExcludeItems.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
        }


    }
}
