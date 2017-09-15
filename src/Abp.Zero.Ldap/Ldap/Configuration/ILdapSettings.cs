using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace Abp.Zero.Ldap.Configuration
{
    /// <summary>
    /// Used to obtain current values of LDAP settings.
    /// This abstraction allows to define a different source for settings than SettingManager (see default implementation: <see cref="LdapSettings"/>).
    /// </summary>
    public interface ILdapSettings
    {
        Task<bool> GetIsEnabled(int? tenantId);

        Task<ContextType> GetContextType(int? tenantId);

        Task<string> GetContainer(int? tenantId);

        Task<string> GetDomain(int? tenantId);

        Task<string> GetUserName(int? tenantId);

        Task<string> GetPassword(int? tenantId);
    }
}