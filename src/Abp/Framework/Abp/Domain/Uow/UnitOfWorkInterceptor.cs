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
                //TODO@Halil: May upgrade to transaction fron non-transactional! Test for EF/NH and implement
                //There is already a running unit of work
                invocation.Proceed();
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
                        UnitOfWorkScope.CurrentUow.Commit();
                    }
                    catch
                    {
                        try { UnitOfWorkScope.CurrentUow.Rollback(); }
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