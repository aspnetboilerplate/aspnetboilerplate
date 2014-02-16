using System.Linq;
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

        public static void Authorize(IInvocation invocation)
        {
            if (!invocation.MethodInvocationTarget.IsDefined(typeof(AbpAuthorizeAttribute), true))
            {
                return;
            }

            var authorizeAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbpAuthorizeAttribute), true);
            AuthorizeAttributeHelper.Authorize(authorizeAttributes.Cast<IAbpAuthorizeAttribute>());
        }
    }
}
