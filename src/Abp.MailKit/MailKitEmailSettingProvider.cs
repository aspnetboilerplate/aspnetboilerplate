using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using Abp.Net.Mail;

namespace Abp.MailKit
{
    /// <summary>
    /// Defines settings to send emails.
    /// <see cref="MailKitEmailSettingNames"/> for all available configurations.
    /// </summary>
    public class MailKitEmailSettingProvider : EmailSettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            var settings = new List<SettingDefinition>(base.GetSettingDefinitions(context));
            settings.AddRange(
                new[]
                   {
                       new SettingDefinition(MailKitEmailSettingNames.Smtp.SecureSocketOption, null, L("SecureSocketOption"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(MailKitEmailSettingNames.Smtp.CheckCertificateRevocation, null, L("CheckCertificateRevocation"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(MailKitEmailSettingNames.Smtp.DisableCertificateValidation, null, L("DisableCertificateValidation"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                   }
                );

            return settings;
        }

    }
}
