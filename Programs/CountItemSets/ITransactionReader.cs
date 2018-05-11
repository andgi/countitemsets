using System;
using System.Collections.Generic;

namespace CountItemSets
{
    public interface ITransactionReader
    {
        void SetMaxNrTransactions(int maxNrTransactions);
        void Begin();
        List<TransactionReader.Transaction> ReadList(int maxLength);
    }
}
