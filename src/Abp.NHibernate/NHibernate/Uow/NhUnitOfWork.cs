using System.Data;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Transactions.Extensions;
using NHibernate;

namespace Abp.NHibernate.Uow
{
    /// <summary>
    /// Implements Unit of work for NHibernate.
    /// </summary>
    public class NhUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        /// <summary>
        /// Gets NHibernate session object to perform queries.
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        /// <see cref="NhUnitOfWork"/> uses this DbConnection if it's set.
        /// This is usually set in tests.
        /// </summary>
        public IDbConnection DbConnection { get; set; }

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
            Session = DbConnection != null
                ? _sessionFactory.OpenSession(DbConnection)
                : _sessionFactory.OpenSession();

            if (Options.IsTransactional == true)
            {
                _transaction = Options.IsolationLevel.HasValue
                    ? Session.BeginTransaction(Options.IsolationLevel.Value.ToSystemDataIsolationLevel())
                    : Session.BeginTransaction();
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