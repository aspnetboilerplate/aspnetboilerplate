using System.Linq;
using Castle.DynamicProxy;

namespace Abp.Application.Authorization
{
    internal static class AuthorizationInterceptionHelper
    {
        public static void Authorize<T>(IInvocation invocation) where T : IAbpAuthorizeAttribute
        {
            if (!invocation.MethodInvocationTarget.IsDefined(typeof(T), true))
            {
                return;
            }

            var authorizeAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(T), true);
            AuthorizeAttributeHelper.Authorize(authorizeAttributes.Cast<IAbpAuthorizeAttribute>());
        }
    }
}