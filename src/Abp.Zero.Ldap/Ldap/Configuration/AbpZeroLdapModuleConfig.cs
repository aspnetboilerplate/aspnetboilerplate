using System;
using Abp.Zero.Configuration;

namespace Abp.Zero.Ldap.Configuration
{
    public class AbpZeroLdapModuleConfig : IAbpZeroLdapModuleConfig
    {
        public bool IsEnabled { get; private set; }

        public Type AuthenticationSourceType { get; private set; }

        private readonly IAbpZeroConfig _zeroConfig;

        public AbpZeroLdapModuleConfig(IAbpZeroConfig zeroConfig)
        {
            _zeroConfig = zeroConfig;
        }

        public void Enable(Type authenticationSourceType)
        {
            AuthenticationSourceType = authenticationSourceType;
            IsEnabled = true;

            _zeroConfig.UserManagement.ExternalAuthenticationSources.Add(authenticationSourceType);
        }
    }
}