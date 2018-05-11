using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace CountItemSets
{
    public class TransactionReader : ITransactionReader
    {
        public class Transaction
        {
            public List<long> EANCodes { get; set; }
            public List<long> VGRCodes { get; set; }
            public Transaction()
            {
                EANCodes = new List<long>(10);
                VGRCodes = new List<long>(10);
            }

            public Transaction(Transaction obj)
            {
                EANCodes = new List<long>(obj.EANCodes);
                VGRCodes = new List<long>(obj.VGRCodes);
            }
        }

        private Transaction current = new Transaction();
        private string fileName;
        private int transactionCount;
        private StreamReader reader;
        private bool endOfFile;
        private string lastLine = null;
        private int lastTransNr = 0;
        private int maxNrTransactions = 1000000000;
        private Dictionary<long, int> dictionaryEANtoVGR;
        public TransactionReader(string fileName, Dictionary<long, int> dictionaryEANtoVGR)
        {
            this.fileName = fileName;
            this.dictionaryEANtoVGR = dictionaryEANtoVGR;
        }
        public void SetMaxNrTransactions(int maxNrTransactions)
        {
            this.maxNrTransactions = maxNrTransactions;
        }
        
        public void Begin() 
        {
            transactionCount = 0;
            try
            {
                reader = new StreamReader(fileName);
                endOfFile = false;
                // Read column headers 
                // TODO: Handle names to index mapping
                if (!reader.EndOfStream)
                    reader.ReadLine();
                // Read first transaction row
                if (!reader.EndOfStream)
                {
                    lastLine = reader.ReadLine();
                    String[] columns = lastLine.Split(';');
                    lastTransNr = Int32.Parse(columns[0]);
                }
                else
                {
                    lastLine = null;
                    endOfFile = true;
                }
            }
            catch (Exception)
            {
                if (reader != null)
                    reader.Close();
                endOfFile = true;
            }
        }

        public bool Read()
        {
            if (lastLine == null)
                endOfFile = true;
            if (endOfFile)
                return false;
            transactionCount++;
            current.EANCodes.Clear();
            current.VGRCodes.Clear();
            while (lastLine != null)
            {
                try
                {
                    String[] columns = lastLine.Split(';');
                    int transNr = Int32.Parse(columns[0]);
                    if (transNr != lastTransNr)
                    {
                        lastTransNr = transNr;
                        break;
                    }

                    long eanNr = Int64.Parse(columns[1]);
                    int vgrNr = Int32.Parse(columns[2]);
                    if (!dictionaryEANtoVGR.ContainsKey(eanNr))
                        dictionaryEANtoVGR.Add(eanNr, vgrNr);
                    current.VGRCodes.Add(-vgrNr);
                    current.EANCodes.Add(eanNr);
                }
                catch (Exception) { }
                if (!reader.EndOfStream && transactionCount < maxNrTransactions)
                    lastLine = reader.ReadLine();
                else
                {
                    reader.Close();
                    lastLine = null;
                }
            }
            current.EANCodes.Sort();
            current.EANCodes = new List<long>(current.EANCodes.Distinct());
            current.VGRCodes.Sort();
            current.VGRCodes = new List<long>(current.VGRCodes.Distinct());
            return true;
        }

        private Transaction ReadTransaction()
        {
            if (lastLine == null)
                endOfFile = true;
            if (endOfFile)
                return null;
            Transaction destination = new Transaction();
            transactionCount++;
            while (lastLine != null)
            {
                try
                {
                    String[] columns = lastLine.Split(';');
                    int transNr = Int32.Parse(columns[0]);
                    if (transNr != lastTransNr)
                    {
                        lastTransNr = transNr;
                        break;
                    }

                    long eanNr = Int64.Parse(columns[1]);
                    int vgrNr = Int32.Parse(columns[2]);
                    if (!dictionaryEANtoVGR.ContainsKey(eanNr))
                        dictionaryEANtoVGR.Add(eanNr, vgrNr);
                    destination.VGRCodes.Add(-vgrNr);
                    destination.EANCodes.Add(eanNr);
                }
                catch (Exception) { }
                if (!reader.EndOfStream && transactionCount < maxNrTransactions)
                    lastLine = reader.ReadLine();
                else
                {
                    reader.Close();
                    lastLine = null;
                }
            }
            return destination;
        }

        public List<Transaction> ReadList(int maxLength)
        {
            List<Transaction> list = new List<Transaction>();
            Transaction transaction;
            while ((transaction = ReadTransaction()) != null)
            {
                list.Add(transaction);
                if (list.Count >= maxLength)
                    break;
            }
            if (list.Count == 0)
                return null;
            Parallel.ForEach(Partitioner.Create(0, list.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    list[i].EANCodes = list[i].EANCodes.Distinct().ToList();
                    list[i].VGRCodes = list[i].VGRCodes.Distinct().ToList();
                    list[i].EANCodes.Sort();
                    list[i].VGRCodes.Sort();
                }
            });
            return list;
        }

        public Transaction Current
        {
            get
            {
                if (!endOfFile)
                    return current;
                else
                    return null;
            }
        }
    }
}
