using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountItemSets
{
    public class FakeTransactionReader : ITransactionReader
    {
        public int NoTransactions {
            get {
                return transactions.Count;
            }
        }

        public ICollection<long> AllEANs {
            get {
                return transactions.
                           SelectMany(t => t.EANCodes).
                           OrderBy(ean => ean).Distinct().ToList();
            }
        }

        public IDictionary<long, int> EANFrequencies {
            get {
                return transactions.
                           SelectMany(t => t.EANCodes).
                           GroupBy(ean => ean).
                           ToDictionary(group => group.Key,
                                        group => group.Count());
            }
        }

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

        public FakeTransactionReader()
        {
            transactions = new List<TransactionReader.Transaction>(1);
            for (int i = 0; i < 1000; i++) {
                TransactionReader.Transaction transaction = new TransactionReader.Transaction();
                long baseEAN = i % 500;
                transaction.EANCodes.InsertRange(0, new long[]{baseEAN + 1, baseEAN + 2, baseEAN + 3 });
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
