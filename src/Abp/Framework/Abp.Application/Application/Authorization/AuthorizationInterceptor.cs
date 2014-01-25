using Abp.Dependency;
using Castle.Core.Logging;
using Castle.DynamicProxy;

namespace Abp.Application.Authorization
{
    /// <summary>
    /// This class is used to intercept methods to make authorization if the method defined <see cref="AbpAuthorizeAttribute"/>.
    /// </summary>
    internal class AuthorizationInterceptor : IInterceptor, ITransientDependency
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.IsDefined(typeof(AbpAuthorizeAttribute), true))
            {
                Authorize(invocation);
            }

            invocation.Proceed();
        }

        private void Authorize(IInvocation invocation)
        {
            using (var authorizationService = IocHelper.ResolveAsDisposable<IAuthorizationService>())
            {
                var authorizeAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbpAuthorizeAttribute), true);
                foreach (AbpAuthorizeAttribute authorizeAttribute in authorizeAttributes)
                {
                    if (authorizeAttribute.RequireAll)
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
                            throw new AbpAuthorizationException("Required permissions are not granted. At lease one of these permissions must be granted: " + requiredPermissions);
                        }
                    }
                }
            }
        }
    }
}
