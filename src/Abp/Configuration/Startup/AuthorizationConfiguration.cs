using Abp.Authorization;
using Abp.Collections;

namespace Abp.Configuration.Startup
{
    internal class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public ITypeList<AuthorizationProvider> Providers { get; private set; }

        public AuthorizationConfiguration()
        {
            Providers = new TypeList<AuthorizationProvider>();
        }
    }
}