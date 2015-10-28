using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.Zero.Ldap.Configuration
{
    /// <summary>
    /// Implements <see cref="ILdapSettings"/> to get settings from <see cref="ISettingManager"/>.
    /// </summary>
    
    public class LdapSettings : ILdapSettings, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public LdapSettings(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public Task<bool> GetIsEnabled(int? tenantId)
        {
            return tenantId.HasValue
                ? _settingManager.GetSettingValueForTenantAsync<bool>(LdapSettingNames.IsEnabled, tenantId.Value)
                : _settingManager.GetSettingValueForApplicationAsync<bool>(LdapSettingNames.IsEnabled);
        }

        public async Task<ContextType> GetContextType(int? tenantId)
        {
            return tenantId.HasValue
                ? (await _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.ContextType, tenantId.Value)).ToEnum<ContextType>()
                : (await _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.ContextType)).ToEnum<ContextType>();
        }

        public Task<string> GetContainer(int? tenantId)
        {
            return tenantId.HasValue
                ? _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.Container, tenantId.Value)
                : _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Container);
        }

        public Task<string> GetDomain(int? tenantId)
        {
            return tenantId.HasValue
                ? _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.Domain, tenantId.Value)
                : _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Domain);
        }

        public Task<string> GetUserName(int? tenantId)
        {
            return tenantId.HasValue
                ? _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.UserName, tenantId.Value)
                : _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.UserName);
        }

        public Task<string> GetPassword(int? tenantId)
        {
            return tenantId.HasValue
                ? _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.Password, tenantId.Value)
                : _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Password);
        }
    }
}