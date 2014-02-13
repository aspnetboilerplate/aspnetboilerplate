using Abp.Dependency;
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

            using (var authorizationService = IocHelper.ResolveAsDisposable<IAuthorizationService>())
            {
                var authorizeAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(T), true);
                foreach (IAbpAuthorizeAttribute authorizeAttribute in authorizeAttributes)
                {
                    if (authorizeAttribute.RequireAllPermissions)
                    {
                        if (!authorizationService.Object.HasAllOfPermissions(authorizeAttribute.Permissions))
                        {
                            var requiredPermissions = string.Join(", ", authorizeAttribute.Permissions);
                            throw new AbpAuthorizationException("Required permissions are not granted. All of these permissions must be granted: " + requiredPermissions);
                        }
                    }
                    else
                    {
                        if (!authorizationService.Object.HasAnyOfPermissions(authorizeAttribute.Permissions))
                        {
                            var requiredPermissions = string.Join(", ", authorizeAttribute.Permissions);
                            throw new AbpAuthorizationException("Required permissions are not granted. At least one of these permissions must be granted: " + requiredPermissions);
                        }
                    }
                }
            }
        }
    }
}