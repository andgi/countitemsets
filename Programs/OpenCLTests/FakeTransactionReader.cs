using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountItemSets
{
    public class FakeTransactionReader : ITransactionReader
    {
        /// <summary>
        ///  The number of transactions.
        /// </summary>
        public int NoTransactions {
            get {
                return transactions.Count;
            }
        }

        /// <summary>
        ///  The collection of all EANs in the transactions.
        /// </summary>
        public ICollection<long> AllEANs {
            get {
                return transactions.
                           SelectMany(t => t.EANCodes).
                           OrderBy(ean => ean).Distinct().ToList();
            }
        }

        /// <summary>
        ///  The frequency of each EAN in the transactions.
        /// </summary>
        public IDictionary<long, int> EANFrequencies {
            get {
                return transactions.
                           SelectMany(t => t.EANCodes).
                           GroupBy(ean => ean).
                           ToDictionary(group => group.Key,
                                        group => group.Count());
            }
        }

        /// <summary>
        ///  The frequency of each pair/2-tuple (within a transaction) of EANs in the transactions.
        /// </summary>
        public IDictionary<string, int> EAN2TupleFrequencies {
            get {
                return transactions.SelectMany
                           (t => t.EANCodes.Join(t.EANCodes,
                                                 a => true, b => true,
                                                 (a, b) => ((a < b) ? "" + a + "," + b : ""))).
                           Where(key => key.Count() > 0).
                           GroupBy(key => key).
                           ToDictionary(group => group.Key,
                                        group => group.Count());
            }
        }

        /// <summary>
        ///  The frequency of each 3-tuple (within a transaction) of EANs in the transactions.
        /// </summary>
        public IDictionary<string, int> EAN3TupleFrequencies {
            get {
                return transactions.SelectMany
                           (t => t.EANCodes.
                                     Join(t.EANCodes,
                                          a => true, b => true,
                                          (a, b) => (a < b) ? new List<long> { a, b } : null).
                                     Where(ab => ab != null).
                                     Join(t.EANCodes,
                                          ab => true, c => true,
                                          (ab, c) => (ab.Last() < c)
                                                     ? ab.Concat(new List<long> { c }).ToArray() : null)).
                           Where(key => key != null).
                           Select(key => "" + key[0] + "," + key[1] + "," + key[2]).
                           GroupBy(key => key).
                           ToDictionary(group => group.Key,
                                        group => group.Count());
            }
        }

        public FakeTransactionReader() : this(400, 20, 3)
        { }

        public FakeTransactionReader(int noTransactions, int noUniqueEANs, int maxTransactionSize)
        {
            transactions = new List<TransactionReader.Transaction>(noTransactions);
            for (int i = 0; i < noTransactions; i++) {
                TransactionReader.Transaction transaction = new TransactionReader.Transaction();
                for (int j = 0; j < maxTransactionSize; j++) {
                    long EAN = (i+j) % noUniqueEANs;
                    transaction.EANCodes.Add(EAN);
                }
                transactions.Add(transaction);
            }
        }

        public void SetMaxNrTransactions(int maxNrTransactions)
        {
        }

        public void Begin()
        {
            read = false;
        }

        public List<TransactionReader.Transaction> ReadList(int maxLength)
        {
            if (!read) {
                read = true;
                return transactions;
            } else {
                return null;
            }
        }

        private bool read = false;
        private List<TransactionReader.Transaction> transactions;
    }
}
