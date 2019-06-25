using System.Reflection;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Threading;
using Castle.DynamicProxy;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This interceptor is used to manage database connection and transactions.
    /// </summary>
    internal class UnitOfWorkInterceptor : CastleAbpInterceptorAdapter<UnitOfWorkInterceptor>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUnitOfWorkDefaultOptions _unitOfWorkOptions;

        public UnitOfWorkInterceptor(IUnitOfWorkManager unitOfWorkManager, IUnitOfWorkDefaultOptions unitOfWorkOptions)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _unitOfWorkOptions = unitOfWorkOptions;
        }

        protected bool ShouldIntercept(IAbpMethodInvocation invocation)
        {
            var unitOfWorkAttr = _unitOfWorkOptions.GetUnitOfWorkAttributeOrNull(invocation.Method);
            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                return false;
            }

            return true;
        }

        private UnitOfWorkAttribute GetUnitOfWorkAttribute(IAbpMethodInvocation invocation)
        {
            var method = invocation.GetMethodInvocationTarget();

            var unitOfWorkAttr = _unitOfWorkOptions.GetUnitOfWorkAttributeOrNull(method);
            return unitOfWorkAttr;
        }

        protected override void InterceptSync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                invocation.Proceed();
                return;
            }

            var unitOfWorkAttr = GetUnitOfWorkAttribute(invocation);

            using (var uow = _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                invocation.Proceed();
                uow.Complete();
            }
        }

        protected override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                await invocation.ProceedAsync();
                return;
            }

            var unitOfWorkAttr = GetUnitOfWorkAttribute(invocation);
            var uow = _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions());

            try
            {
                await invocation.ProceedAsync();
            }
            catch
            {
                uow.Dispose();
                throw;
            }

            try
            {
                await uow.CompleteAsync();
            }
            finally
            {
                uow.Dispose();
            }
        }
    }
}