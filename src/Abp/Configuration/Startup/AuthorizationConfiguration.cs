using Abp.Authorization;
using Abp.Collections;

namespace Abp.Configuration.Startup
{
    internal class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public AuthorizationConfiguration()
        {
            Providers = new TypeList<AuthorizationProvider>();
        }

        public ITypeList<AuthorizationProvider> Providers { get; }
    }
}