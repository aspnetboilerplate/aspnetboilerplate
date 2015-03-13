using System.Collections.Generic;
using System.Threading.Tasks;
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
                await PermissionChecker.AuthorizeAsync(authorizeAttribute.RequireAllPermissions, authorizeAttribute.Permissions);
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