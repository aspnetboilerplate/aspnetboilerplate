using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.Threading;

namespace Abp.Authorization
{
    internal class AuthorizeAttributeHelper : IAuthorizeAttributeHelper, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        public IPermissionChecker PermissionChecker { get; set; }

        public AuthorizeAttributeHelper()
        {
            AbpSession = NullAbpSession.Instance;
            PermissionChecker = NullPermissionChecker.Instance;
        }

        public async Task AuthorizeAsync(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
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
                    foreach (var permissionName in authorizeAttribute.Permissions)
                    {
                        if (!await PermissionChecker.IsGrantedAsync(permissionName))
                        {
                            throw new AbpAuthorizationException(
                                "Required permissions are not granted. All of these permissions must be granted: " +
                                String.Join(", ", authorizeAttribute.Permissions)
                                );
                        }
                    }
                }
                else
                {
                    foreach (var permissionName in authorizeAttribute.Permissions)
                    {
                        if (await PermissionChecker.IsGrantedAsync(permissionName))
                        {
                            return; //Authorized
                        }
                    }

                    //Not authorized!
                    throw new AbpAuthorizationException(
                        "Required permissions are not granted. At least one of these permissions must be granted: " +
                        String.Join(", ", authorizeAttribute.Permissions)
                        );
                }
            }
        }

        public async Task AuthorizeAsync(IAbpAuthorizeAttribute authorizeAttribute)
        {
            await AuthorizeAsync(new[] { authorizeAttribute });
        }

        public void Authorize(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
        {
            AsyncHelper.RunSync(() => AuthorizeAsync(authorizeAttributes));
        }

        public void Authorize(IAbpAuthorizeAttribute authorizeAttribute)
        {
            Authorize(new[] { authorizeAttribute });
        }
    }
}