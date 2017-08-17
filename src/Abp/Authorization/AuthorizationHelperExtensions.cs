using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Threading;

namespace Abp.Authorization
{
    public static class AuthorizationHelperExtensions
    {
        public static async Task AuthorizeAsync(this IAuthorizationHelper authorizationHelper, IAbpAuthorizeAttribute authorizeAttribute)
        {
            await authorizationHelper.AuthorizeAsync(new[] { authorizeAttribute });
        }

        public static void Authorize(this IAuthorizationHelper authorizationHelper, IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
        {
            AsyncHelper.RunSync(() => authorizationHelper.AuthorizeAsync(authorizeAttributes));
        }

        public static void Authorize(this IAuthorizationHelper authorizationHelper, IAbpAuthorizeAttribute authorizeAttribute)
        {
            authorizationHelper.Authorize(new[] { authorizeAttribute });
        }

        public static void Authorize(this IAuthorizationHelper authorizationHelper, MethodInfo methodInfo)
        {
            AsyncHelper.RunSync(() => authorizationHelper.AuthorizeAsync(methodInfo));
        }
    }
}