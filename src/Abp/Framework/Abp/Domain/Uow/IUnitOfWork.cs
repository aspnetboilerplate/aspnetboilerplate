namespace Abp.Domain.Uow
{
    /// <summary>
    /// Represents a transactional job.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        void Rollback();
    }
}