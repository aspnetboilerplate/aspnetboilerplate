using Abp.Authorization;
using Abp.Collections;

namespace Abp.Configuration.Startup
{
    internal class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public ITypeList<AuthorizationProvider> Providers { get; private set; }

        public bool IsEnabled { get; set; }

        public AuthorizationConfiguration()
        {
            Providers = new TypeList<AuthorizationProvider>();
            IsEnabled = true;
        }
    }
}