using System.Threading.Tasks;
using Abp.Aspects;
using Abp.Dependency;

namespace Abp.Runtime.Validation.Interception
{
    /// <summary>
    /// This interceptor is used intercept method calls for classes which's methods must be validated.
    /// </summary>
    public class ValidationInterceptor : CastleAbpInterceptorAdapter<ValidationInterceptor>
    {
        private readonly IIocResolver _iocResolver;

        public ValidationInterceptor(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        protected override void InterceptSync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                invocation.Proceed();
                return;
            }

            using (var validator = _iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.GetMethodInvocationTarget(), invocation.Arguments);
                validator.Object.Validate();
            }

            invocation.Proceed();
        }

        protected override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                await invocation.ProceedAsync();
                return;
            }

            using (var validator = _iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.GetMethodInvocationTarget(), invocation.Arguments);
                validator.Object.Validate();
            }

            await invocation.ProceedAsync();
        }

        protected bool ShouldIntercept(IAbpMethodInvocation invocation)
        {
            return AbpCrossCuttingConcerns.IsApplied(invocation.GetMethodInvocationTarget(), AbpCrossCuttingConcerns.Validation);
        }
    }
}
