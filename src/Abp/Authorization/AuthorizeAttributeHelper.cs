using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.Authorization
{
    //TODO: Implement Async!
    internal class AuthorizeAttributeHelper : ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        public IPermissionChecker PermissionChecker { get; set; }

        public AuthorizeAttributeHelper()
        {
            AbpSession = NullAbpSession.Instance;
            PermissionChecker = NullPermissionChecker.Instance;
        }

        public void Authorize(IAbpAuthorizeAttribute authorizeAttribute)
        {
            Authorize(new[] { authorizeAttribute });
        }

        public void Authorize(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
        {
            if (!AbpSession.UserId.HasValue)
            {
                throw new AbpAuthorizationException("No user logged in!");
            }

            foreach (var authorizeAttribute in authorizeAttributes)
            {
                if (authorizeAttribute.Permissions.IsNullOrEmpty())
                {
                    continue;
                }

                if (authorizeAttribute.RequireAllPermissions)
                {
                    if (!authorizeAttribute.Permissions.All(permissionName => PermissionChecker.IsGranted(permissionName)))
                    {
                        throw new AbpAuthorizationException(
                            "Required permissions are not granted. All of these permissions must be granted: " +
                            String.Join(", ", authorizeAttribute.Permissions)
                            );
                    }
                }
                else
                {
                    if (!authorizeAttribute.Permissions.Any(permissionName => PermissionChecker.IsGranted(permissionName)))
                    {
                        throw new AbpAuthorizationException(
                            "Required permissions are not granted. At least one of these permissions must be granted: " +
                            String.Join(", ", authorizeAttribute.Permissions)
                            );
                    }
                }
            }
        }
    }
}