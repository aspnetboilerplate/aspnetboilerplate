using Abp.Dependency;
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
        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            //If there isalready  a running transaction (or no need to a db connection), just run the method
            if (UnitOfWorkScope.Current != null || !UnitOfWorkHelper.ShouldPerformUnitOfWork(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            using (var unitOfWork = IocHelper.ResolveAsDisposable<IUnitOfWork>())
            {
                try
                {

                    UnitOfWorkScope.Current = unitOfWork.Object;
                    UnitOfWorkScope.Current.BeginTransaction();

                    try
                    {
                        invocation.Proceed();
                        UnitOfWorkScope.Current.Commit();
                    }
                    catch
                    {
                        try { UnitOfWorkScope.Current.Rollback(); } catch { }
                        throw;
                    }
                }
                finally
                {
                    UnitOfWorkScope.Current = null;
                }
            }
        }
    }
}