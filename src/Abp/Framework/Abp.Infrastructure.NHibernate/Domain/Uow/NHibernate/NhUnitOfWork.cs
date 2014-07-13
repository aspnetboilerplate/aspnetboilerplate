using NHibernate;

namespace Abp.Domain.Uow.NHibernate
{
    /// <summary>
    /// Implements Unit of work for NHibernate.
    /// </summary>
    public class NhUnitOfWork : UnitOfWorkBase
    {
        /// <summary>
        /// Gets Nhibernate session object to perform queries.
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        /// Reference to the session factory.
        /// </summary>
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// Reference to the currently running transcation.
        /// </summary>
        private ITransaction _transaction;

        /// <summary>
        /// Is this object disposed?
        /// Used to prevent multiple dispose.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of NhUnitOfWork.
        /// </summary>
        /// <param name="sessionFactory"></param>
        public NhUnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        public override void Begin()
        {
            Session = _sessionFactory.OpenSession();
            if (IsTransactional)
            {
                _transaction = Session.BeginTransaction();                
            }
        }

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        public override void End()
        {
            try
            {
                Session.Flush();
                if (_transaction != null)
                {
                    _transaction.Commit();                    
                }

                TriggerSuccessHandlers();
            }
            finally
            {
                Dispose();
            }
        }

        public override void Cancel()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }
            }
            finally 
            {
                Dispose();
            }
        }

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        public override void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
            finally
            {
                Session.Dispose();                
            }
        }
    }
}