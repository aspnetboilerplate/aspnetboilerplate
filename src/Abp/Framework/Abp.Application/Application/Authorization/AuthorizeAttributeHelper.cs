using System;
using System.Collections.Generic;
using Abp.Authorization;
using Abp.Collections;
using Abp.Dependency;

namespace Abp.Application.Authorization
{
    public class AuthorizeAttributeHelper : ITransientDependency //TODO: Make internal
    {
        public IAuthorizationService AuthorizationService { get; set; }

        public AuthorizeAttributeHelper()
        {
            AuthorizationService = NullAuthorizationService.Instance;            
        }

        public void Authorize(IAbpAuthorizeAttribute authorizeAttribute)
        {
            Authorize(new[] { authorizeAttribute });
        }

        public void Authorize(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
        {
            foreach (var authorizeAttribute in authorizeAttributes)
            {
                if (authorizeAttribute.Permissions.IsNullOrEmpty())
                {
                    continue;
                }

                if (authorizeAttribute.RequireAllPermissions)
                {
                    if (!AuthorizationService.HasAllOfPermissions(authorizeAttribute.Permissions))
                    {
                        var requiredPermissions = String.Join(", ", authorizeAttribute.Permissions);
                        throw new AbpAuthorizationException("Required permissions are not granted. All of these permissions must be granted: " + requiredPermissions);
                    }
                }
                else
                {
                    if (!AuthorizationService.HasAnyOfPermissions(authorizeAttribute.Permissions))
                    {
                        var requiredPermissions = String.Join(", ", authorizeAttribute.Permissions);
                        throw new AbpAuthorizationException("Required permissions are not granted. At least one of these permissions must be granted: " + requiredPermissions);
                    }
                }
            }
        }
    }
}