using Abp.Dependency;
using Castle.DynamicProxy;
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
            if (UnitOfWorkScope.CurrentUow != null || !UnitOfWorkHelper.ShouldPerformUnitOfWork(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            using (var unitOfWork = IocHelper.ResolveAsDisposable<IUnitOfWork>())
            {
                try
                {

                    UnitOfWorkScope.CurrentUow = unitOfWork.Object;
                    UnitOfWorkScope.CurrentUow.BeginTransaction();

                    try
                    {
                        invocation.Proceed();
                        UnitOfWorkScope.CurrentUow.Commit();
                    }
                    catch
                    {
                        try { UnitOfWorkScope.CurrentUow.Rollback(); } catch { }
                        throw;
                    }
                }
                finally
                {
                    UnitOfWorkScope.CurrentUow = null;
                }
            }
        }
    }
}