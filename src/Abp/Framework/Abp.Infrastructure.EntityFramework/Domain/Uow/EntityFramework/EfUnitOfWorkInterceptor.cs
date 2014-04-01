using System;
using Abp.Dependency;
using Castle.DynamicProxy;

namespace Abp.Domain.Uow.EntityFramework
{
    /// <summary>
    /// This interceptor is used to manage database connection and transactions.
    /// </summary>
    public class EfUnitOfWorkInterceptor : IInterceptor
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
                    catch(Exception ex)
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