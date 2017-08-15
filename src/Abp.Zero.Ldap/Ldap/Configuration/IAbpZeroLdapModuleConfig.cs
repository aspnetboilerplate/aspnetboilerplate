using System;

namespace Abp.Zero.Ldap.Configuration
{
    public interface IAbpZeroLdapModuleConfig
    {
        bool IsEnabled { get; }

        Type AuthenticationSourceType { get; }

        void Enable(Type authenticationSourceType);
    }
}