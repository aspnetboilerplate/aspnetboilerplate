using Abp.Dependency;
using Castle.DynamicProxy;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This interceptor is used to manage database connection and transactions.
    /// </summary>
    public class UnitOfWorkInterceptor : IInterceptor
    {
        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            if (UnitOfWorkScope.CurrentUow != null)
            {
                //There is already a running unit of work
                //var unitOfWorkAttr1 = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(invocation.MethodInvocationTarget);
                //var startedTransaction = false;
                //if (unitOfWorkAttr1 != null && unitOfWorkAttr1.IsTransactional)
                //{
                //    startedTransaction = UnitOfWorkScope.CurrentUow.StartTransaction();
                //}
                
                invocation.Proceed();

                //if (startedTransaction)
                //{
                //    UnitOfWorkScope.CurrentUow.CommitTransaction();
                //}

                return;
            }

            var unitOfWorkAttr = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(invocation.MethodInvocationTarget);
            if (unitOfWorkAttr == null)
            {
                if (!UnitOfWorkHelper.IsConventionalUowClass(invocation.MethodInvocationTarget.DeclaringType))
                {
                    //UnitOfWork is not defined and this is not a conventional unit-of-work class
                    invocation.Proceed();
                    return;
                }

                unitOfWorkAttr = new UnitOfWorkAttribute();
            }
            else if (unitOfWorkAttr.IsDisabled)
            {
                //Disabled unit of work
                invocation.Proceed();
                return;
            }

            using (var unitOfWork = IocHelper.ResolveAsDisposable<IUnitOfWork>())
            {
                try
                {
                    UnitOfWorkScope.CurrentUow = unitOfWork.Object;
                    UnitOfWorkScope.CurrentUow.Begin(unitOfWorkAttr.IsTransactional);
                    try
                    {
                        invocation.Proceed();
                        UnitOfWorkScope.CurrentUow.End();
                    }
                    catch
                    {
                        try { UnitOfWorkScope.CurrentUow.Cancel(); }
                        catch { }
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