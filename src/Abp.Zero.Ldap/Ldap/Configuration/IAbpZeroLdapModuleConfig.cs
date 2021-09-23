using System;

namespace Abp.Zero.Ldap.Configuration
{
    public interface IAbpZeroLdapModuleConfig
    {
        bool IsEnabled { get; }
        
        /// <summary>
        /// Otherwise SamAccountName will be used as a username
        /// </summary>
        bool UseUserPrincipalNameAsUserName { get; set; }

        Type AuthenticationSourceType { get; }

        void Enable(Type authenticationSourceType);
    }
}