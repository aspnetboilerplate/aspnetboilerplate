using System.Data;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Transactions;
using NHibernate;

namespace Abp.NHibernate.Uow
{
    /// <summary>
    /// Implements Unit of work for NHibernate.
    /// </summary>
    public class NhUnitOfWork : UnitOfWorkBase
    {
        /// <summary>
        /// Gets NHibernate session object to perform queries.
        /// </summary>
        public ISession Session { get; private set; }

        private readonly ISessionFactory _sessionFactory;
        private ITransaction _transaction;

        /// <summary>
        /// Creates a new instance of <see cref="NhUnitOfWork"/>.
        /// </summary>
        public NhUnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        protected override void BeginUow()
        {
            Session = _sessionFactory.OpenSession();
            if (Options.IsTransactional == true) //TODO: Use default value if not provided!
            {
                //TODO: Get default values from somewhere...
                _transaction = Session.BeginTransaction(
                    Options.IsolationLevel.GetValueOrDefault(System.Transactions.IsolationLevel.ReadUncommitted).ToSystemDataIsolationLevel()
                    );
            }
        }

        public override void SaveChanges()
        {
            Session.Flush();
        }

        public async override Task SaveChangesAsync()
        {
            Session.Flush();
        }

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        protected override void CompleteUow()
        {
            SaveChanges();
            if (_transaction != null)
            {
                _transaction.Commit();
            }
        }

        protected async override Task CompleteUowAsync()
        {
            CompleteUow();
        }

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        protected override void DisposeUow()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            Session.Dispose();
        }
    }
}