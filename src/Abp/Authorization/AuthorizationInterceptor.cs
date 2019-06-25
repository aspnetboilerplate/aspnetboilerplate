using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Authorization
{
    /// <summary>
    /// This class is used to intercept methods to make authorization if the method defined <see cref="AbpAuthorizeAttribute"/>.
    /// </summary>
    public class AuthorizationInterceptor : CastleAbpInterceptorAdapter<AuthorizationInterceptor>
    {
        private readonly IAuthorizationHelper _authorizationHelper;

        public AuthorizationInterceptor(IAuthorizationHelper authorizationHelper)
        {
            _authorizationHelper = authorizationHelper;
        }

        protected override void InterceptSync(IAbpMethodInvocation invocation)
        {
            _authorizationHelper.Authorize(invocation.GetMethodInvocationTarget(), invocation.TargetObjectType);
            invocation.Proceed();
        }

        protected override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            _authorizationHelper.Authorize(invocation.GetMethodInvocationTarget(), invocation.TargetObjectType);
            await invocation.ProceedAsync();
        }
    }
}
