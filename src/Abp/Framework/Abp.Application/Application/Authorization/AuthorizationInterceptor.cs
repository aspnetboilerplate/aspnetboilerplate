using System.Linq;
using Abp.Dependency;
using Castle.DynamicProxy;

namespace Abp.Application.Authorization
{
    /// <summary>
    /// This class is used to intercept methods to make authorization if the method defined <see cref="AbpAuthorizeAttribute"/>.
    /// </summary>
    internal class AuthorizationInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Authorize(invocation);
            invocation.Proceed();
        }

        public void Authorize(IInvocation invocation)
        {
            if (!invocation.MethodInvocationTarget.IsDefined(typeof(AbpAuthorizeAttribute), true))
            {
                return;
            }

            var authorizeAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbpAuthorizeAttribute), true);
            using (var authorizationAttributeHelper = IocHelper.ResolveAsDisposable<AuthorizeAttributeHelper>())
            {
                authorizationAttributeHelper.Object.Authorize(authorizeAttributes.Cast<IAbpAuthorizeAttribute>());
            }
        }
    }
}
