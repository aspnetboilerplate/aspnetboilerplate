using System.Data;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Transactions.Extensions;
using NHibernate;

namespace Abp.NHibernate.Uow
{
    /// <summary>
    ///     Implements Unit of work for NHibernate.
    /// </summary>
    public class NhUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        private readonly ISessionFactory _sessionFactory;
        private ITransaction _transaction;

        /// <summary>
        ///     Creates a new instance of <see cref="NhUnitOfWork" />.
        /// </summary>
        public NhUnitOfWork(ISessionFactory sessionFactory, IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkDefaultOptions defaultOptions)
            : base(connectionStringResolver, defaultOptions)
        {
            AbpSession = NullAbpSession.Instance;
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        ///     Used to get current session values.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        ///     Gets NHibernate session object to perform queries.
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        ///     <see cref="NhUnitOfWork" /> uses this DbConnection if it's set.
        ///     This is usually set in tests.
        /// </summary>
        public IDbConnection DbConnection { get; set; }

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

            CheckAndSetMayHaveTenant();
            CheckAndSetMustHaveTenant();
        }

        protected virtual void CheckAndSetMustHaveTenant()
        {
            if (IsFilterEnabled(AbpDataFilters.MustHaveTenant)) return;
            if (AbpSession.TenantId == null) return;
            ApplyEnableFilter(AbpDataFilters.MustHaveTenant); //Enable Filters
            ApplyFilterParameterValue(AbpDataFilters.MustHaveTenant,
                AbpDataFilters.Parameters.TenantId,
                AbpSession.GetTenantId()); //ApplyFilter
        }

        protected virtual void CheckAndSetMayHaveTenant()
        {
            if (IsFilterEnabled(AbpDataFilters.MayHaveTenant)) return;
            if (AbpSession.TenantId == null) return;
            ApplyEnableFilter(AbpDataFilters.MayHaveTenant); //Enable Filters
            ApplyFilterParameterValue(AbpDataFilters.MayHaveTenant,
                AbpDataFilters.Parameters.TenantId,
                AbpSession.TenantId); //ApplyFilter
        }

        public override void SaveChanges()
        {
            Session.Flush();
        }

        public override Task SaveChangesAsync()
        {
            Session.Flush();
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Commits transaction and closes database connection.
        /// </summary>
        protected override void CompleteUow()
        {
            SaveChanges();
            if (_transaction != null)
            {
                _transaction.Commit();
            }
        }

        protected override Task CompleteUowAsync()
        {
            CompleteUow();
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Rollbacks transaction and closes database connection.
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

        protected override void ApplyEnableFilter(string filterName)
        {
            if (Session.GetEnabledFilter(filterName) == null)
                Session.EnableFilter(filterName);
        }

        protected override void ApplyDisableFilter(string filterName)
        {
            if (Session.GetEnabledFilter(filterName) != null)
                Session.DisableFilter(filterName);
        }

        protected override void ApplyFilterParameterValue(string filterName, string parameterName, object value)
        {
            if (Session == null)
            {
                return;
            }

            var filter = Session.GetEnabledFilter(filterName);
            if (filter != null)
            {
                filter.SetParameter(parameterName, value);
            }
        }
    }
}