using Microsoft.AspNet.Identity;

namespace Abp.Zero.AspNetCore
{
    internal class AbpZeroAspNetCoreConfiguration : IAbpZeroAspNetCoreConfiguration
    {
        public string AuthenticationScheme { get; set; }

        public string TwoFactorAuthenticationScheme { get; set; }

        public string TwoFactorRememberBrowserAuthenticationScheme { get; set; }

        public AbpZeroAspNetCoreConfiguration()
        {
            AuthenticationScheme = "AppAuthenticationScheme";
            TwoFactorAuthenticationScheme = DefaultAuthenticationTypes.TwoFactorCookie;
            TwoFactorRememberBrowserAuthenticationScheme = DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie;
        }
    }
}
