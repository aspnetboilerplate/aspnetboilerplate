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
            if (UnitOfWorkScope.Current != null)
            {               
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
                    UnitOfWorkScope.Current = unitOfWork.Object;
                    UnitOfWorkScope.Current.Begin(unitOfWorkAttr.IsTransactional);
                    try
                    {
                        invocation.Proceed();
                        UnitOfWorkScope.Current.End();
                    }
                    catch
                    {
                        try { UnitOfWorkScope.Current.Cancel(); }
                        catch { }
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