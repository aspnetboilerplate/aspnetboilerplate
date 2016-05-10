using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Threading;

namespace Abp.Authorization
{
    internal class AuthorizeAttributeHelper : IAuthorizeAttributeHelper, ITransientDependency
    {
        public AuthorizeAttributeHelper()
        {
            AbpSession = NullAbpSession.Instance;
            PermissionChecker = NullPermissionChecker.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        public IAbpSession AbpSession { get; set; }

        public IPermissionChecker PermissionChecker { get; set; }

        public ILocalizationManager LocalizationManager { get; set; }

        public async Task AuthorizeAsync(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
        {
            if (!AbpSession.UserId.HasValue)
            {
                throw new AbpAuthorizationException(LocalizationManager.GetString(AbpConsts.LocalizationSourceName,
                    "CurrentUserDidNotLoginToTheApplication"));
            }

            foreach (var authorizeAttribute in authorizeAttributes)
            {
                await
                    PermissionChecker.AuthorizeAsync(authorizeAttribute.RequireAllPermissions,
                        authorizeAttribute.Permissions);
            }
        }

        public async Task AuthorizeAsync(IAbpAuthorizeAttribute authorizeAttribute)
        {
            await AuthorizeAsync(new[] {authorizeAttribute});
        }

        public void Authorize(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
        {
            AsyncHelper.RunSync(() => AuthorizeAsync(authorizeAttributes));
        }

        public void Authorize(IAbpAuthorizeAttribute authorizeAttribute)
        {
            Authorize(new[] {authorizeAttribute});
        }
    }
}