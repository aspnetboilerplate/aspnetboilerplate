using System.Threading.Tasks;
using Abp.Domain.Uow;
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

        protected override void StartInternal()
        {
            Session = _sessionFactory.OpenSession();
            if (IsTransactional)
            {
                _transaction = Session.BeginTransaction();                
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
        protected override void CompleteInternal()
        {
            SaveChanges();
            if (_transaction != null)
            {
                _transaction.Commit();
            }
        }

        protected async override Task CompleteInternalAsync()
        {
            CompleteInternal();
        }

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        protected override void DisposeInternal()
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