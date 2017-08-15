using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Extensions;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace Abp.Owin
{
    public static class AbpZeroOwinAppBuilderExtensions
    {
        public static void RegisterDataProtectionProvider(this IAppBuilder app)
        {
            if (!IocManager.Instance.IsRegistered<IUserTokenProviderAccessor>())
            {
                throw new AbpException("IUserTokenProviderAccessor is not registered!");
            }

            var providerAccessor = IocManager.Instance.Resolve<IUserTokenProviderAccessor>();
            if (!(providerAccessor is OwinUserTokenProviderAccessor))
            {
                throw new AbpException($"IUserTokenProviderAccessor should be instance of {nameof(OwinUserTokenProviderAccessor)}!");
            }

            providerAccessor.As<OwinUserTokenProviderAccessor>().DataProtectionProvider = app.GetDataProtectionProvider();
        }
    }
}