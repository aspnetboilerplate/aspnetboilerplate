using System.Collections.Generic;

namespace Abp.Transactions
{
    public class ActiveTransactionProviderArgs : Dictionary<string, object>
    {
        public static ActiveTransactionProviderArgs Empty = new ActiveTransactionProviderArgs();
    }
}
