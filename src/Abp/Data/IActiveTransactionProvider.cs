using System.Data;
using System.Threading.Tasks;

namespace Abp.Data
{
    public interface IActiveTransactionProvider
    {
        /// <summary>
        ///     Gets the active transaction or null if current UOW is not transactional.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<IDbTransaction> GetActiveTransactionAsync(ActiveTransactionProviderArgs args);

        IDbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args);
        
        /// <summary>
        ///     Gets the active database connection.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<IDbConnection> GetActiveConnectionAsync(ActiveTransactionProviderArgs args);
        
        IDbConnection GetActiveConnection(ActiveTransactionProviderArgs args);
    }
}
