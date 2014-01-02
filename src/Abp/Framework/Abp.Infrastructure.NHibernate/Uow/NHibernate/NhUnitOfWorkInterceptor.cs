using Abp.Domain.Repositories.NHibernate;
using Castle.DynamicProxy;
using NHibernate;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace Abp.Domain.Uow.NHibernate
{
    /// <summary>
    /// This interceptor is used to manage database connection and transactions.
    /// </summary>
    public class NhUnitOfWorkInterceptor : IInterceptor
    {
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// Creates a new NhUnitOfWorkInterceptor object.
        /// </summary>
        /// <param name="sessionFactory">Nhibernate session factory.</param>
        public NhUnitOfWorkInterceptor(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            //If there isalready  a running transaction (or no need to a db connection), just run the method
            if (NhUnitOfWork.Current != null || !UnitOfWorkHelper.ShouldPerformUnitOfWork(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            try
            {
                NhUnitOfWork.Current = new NhUnitOfWork(_sessionFactory);
                NhUnitOfWork.Current.BeginTransaction();

                try
                {
                    invocation.Proceed();
                    NhUnitOfWork.Current.Commit();
                }
                catch
                {
                    try { NhUnitOfWork.Current.Rollback(); } catch { }
                    throw;
                }
            }
            finally
            {
                NhUnitOfWork.Current = null;
            }
        }
    }
}