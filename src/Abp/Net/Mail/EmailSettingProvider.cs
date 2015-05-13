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
                       new SettingDefinition(EmailSettingNames.Smtp.Host, "127.0.0.1", L("SmtpHost")),
                       new SettingDefinition(EmailSettingNames.Smtp.Port, "25", L("SmtpPort")),
                       new SettingDefinition(EmailSettingNames.Smtp.UserName, "", L("Username")),
                       new SettingDefinition(EmailSettingNames.Smtp.Password, "", L("Password")),
                       new SettingDefinition(EmailSettingNames.Smtp.Domain, "", L("DomainName")),
                       new SettingDefinition(EmailSettingNames.Smtp.EnableSsl, "false", L("UseSSL")),
                       new SettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials, "true", L("UseDefaultCredentials")),
                       new SettingDefinition(EmailSettingNames.DefaultFromAddress, "", L("DefaultFromSenderEmailAddress")),
                       new SettingDefinition(EmailSettingNames.DefaultFromDisplayName, "", L("DefaultFromSenderDisplayName"))
                   };
        }

        private static LocalizableString L(string name)
        {
            return new LocalizableString(name, AbpConsts.LocalizationSourceName);
        }
    }
}