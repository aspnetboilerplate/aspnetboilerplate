using System;
using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Application.Authorization
{
    internal static class AuthorizeAttributeHelper
    {
        public static void Authorize(IAbpAuthorizeAttribute authorizeAttribute)
        {
            Authorize(new[] { authorizeAttribute });
        }

        public static void Authorize(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
        {
            using (var authorizationService = IocHelper.ResolveAsDisposable<IAuthorizationService>())
            {
                foreach (IAbpAuthorizeAttribute authorizeAttribute in authorizeAttributes)
                {
                    if (authorizeAttribute.RequireAllPermissions)
                    {
                        if (!authorizationService.Object.HasAllOfPermissions(authorizeAttribute.Permissions))
                        {
                            var requiredPermissions = String.Join(", ", authorizeAttribute.Permissions);
                            throw new AbpAuthorizationException("Required permissions are not granted. All of these permissions must be granted: " + requiredPermissions);
                        }
                    }
                    else
                    {
                        if (!authorizationService.Object.HasAnyOfPermissions(authorizeAttribute.Permissions))
                        {
                            var requiredPermissions = String.Join(", ", authorizeAttribute.Permissions);
                            throw new AbpAuthorizationException("Required permissions are not granted. At least one of these permissions must be granted: " + requiredPermissions);
                        }
                    }
                }
            }
        }
    }
}