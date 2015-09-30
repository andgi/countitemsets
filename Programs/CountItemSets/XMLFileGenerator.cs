using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace CountItemSets
{
    public class XMLFileGenerator: IFrequentItemsetGenerator
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

        private int transactionCount; // Should be handled by separate class TransactionReader
        private Dictionary<long, int> dictionaryEANtoVGR = new Dictionary<long, int>(); // Should be handled by separate class TransactionContext

        public void SetPruningMinSupport(double minSupport)
        {
        }

        public void SetMaxNrTransactions(int maxNrTransactions)
        {

        }

        public int GetTransactionCount()
        {
            return transactionCount;
        }

        public int GetProgess()
        {
            return 100;
        }

        public Stopwatch GetStopWatch()
        {
            return null;
        }

        public Dictionary<long, int> GetDictionaryEANtoVGR()
        {
            return dictionaryEANtoVGR;
        }
        public void BeginGenerate(string fileNameTransaction, GenerateCallBack callBack)
        {
        }

        public void Generate(string fileNameTransaction)
        {
        }

        public void Load(string fileNameItemsets)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileNameItemsets);
            XmlNode nodeDictionary = document.FirstChild.NextSibling.ChildNodes[0];
            transactionCount = int.Parse(nodeDictionary.InnerText);
            nodeDictionary = nodeDictionary.NextSibling;
            dictionaryEANtoVGR = new Dictionary<long, int>();
            foreach (XmlNode node in nodeDictionary.ChildNodes)
            {
                long key = long.Parse(node.ChildNodes[0].InnerText);
                int value = int.Parse(node.ChildNodes[1].InnerText);
                dictionaryEANtoVGR.Add(key, value);
            }
            nodeDictionary = nodeDictionary.NextSibling;
            dictionaryLevel1 = new Dictionary<long, int>();
            foreach (XmlNode node in nodeDictionary.ChildNodes)
            {
                long key = long.Parse(node.ChildNodes[0].InnerText);
                int value = int.Parse(node.ChildNodes[1].InnerText);
                dictionaryLevel1.Add(key, value);
            }
            nodeDictionary = nodeDictionary.NextSibling;
            dictionaryLevel2 = new Dictionary<string, int>();
            foreach (XmlNode node in nodeDictionary.ChildNodes)
            {
                string key = node.ChildNodes[0].InnerText;
                int value = int.Parse(node.ChildNodes[1].InnerText);
                dictionaryLevel2.Add(key, value);
            }
            nodeDictionary = nodeDictionary.NextSibling;
            dictionaryLevel3 = new Dictionary<string, int>();
            foreach (XmlNode node in nodeDictionary.ChildNodes)
            {
                string key = node.ChildNodes[0].InnerText;
                int value = int.Parse(node.ChildNodes[1].InnerText);
                dictionaryLevel3.Add(key, value);
            }
            nodeDictionary = nodeDictionary.NextSibling;
            dictionaryLevel4 = new Dictionary<string, int>();
            foreach (XmlNode node in nodeDictionary.ChildNodes)
            {
                string key = node.ChildNodes[0].InnerText;
                int value = int.Parse(node.ChildNodes[1].InnerText);
                dictionaryLevel4.Add(key, value);
            }
            nodeDictionary = nodeDictionary.NextSibling;
            dictionaryLevel5 = new Dictionary<string, int>();
            foreach (XmlNode node in nodeDictionary.ChildNodes)
            {
                string key = node.ChildNodes[0].InnerText;
                int value = int.Parse(node.ChildNodes[1].InnerText);
                dictionaryLevel5.Add(key, value);
            }
        }

    }
}
