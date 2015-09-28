using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CountItemSets
{
    public class TransactionReader
    {
        public class Transaction
        {
            public Transaction()
            {

            }
        }

        private string fileName;
        private int transactionCount;
        private StreamReader reader;
        public TransactionReader(string fileName)
        {
            this.fileName = fileName;
        }
        
        public void Begin() 
        {
            reader = new StreamReader(fileName);
        }

        public bool ReadNext()
        {
            return false;
        }

        public Transaction Current()
        {
            return null;
        }
    }
}
