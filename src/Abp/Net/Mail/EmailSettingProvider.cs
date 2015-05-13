using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;

namespace Abp.Net.Mail
{
    /// <summary>
    /// Defines settings to send emails.
    /// <see cref="EmailSettingNames"/> for all available configurations.
    /// </summary>
    internal class EmailSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                   {
                       new SettingDefinition(EmailSettingNames.Smtp.Host, "127.0.0.1", L("SmtpHost"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(EmailSettingNames.Smtp.Port, "25", L("SmtpPort"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(EmailSettingNames.Smtp.UserName, "", L("Username"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(EmailSettingNames.Smtp.Password, "", L("Password"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(EmailSettingNames.Smtp.Domain, "", L("DomainName"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(EmailSettingNames.Smtp.EnableSsl, "false", L("UseSSL"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials, "true", L("UseDefaultCredentials"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(EmailSettingNames.DefaultFromAddress, "", L("DefaultFromSenderEmailAddress"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(EmailSettingNames.DefaultFromDisplayName, "", L("DefaultFromSenderDisplayName"), scopes: SettingScopes.Application | SettingScopes.Tenant)
                   };
        }

        private static LocalizableString L(string name)
        {
            return new LocalizableString(name, AbpConsts.LocalizationSourceName);
        }
    }
}