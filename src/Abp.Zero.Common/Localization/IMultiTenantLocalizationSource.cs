using System.Collections.Generic;
using System.Globalization;
using Abp.Localization.Sources;

namespace Abp.Localization
{
    /// <summary>
    /// Extends <see cref="ILocalizationSource"/> to add tenant and database based localization.
    /// </summary>
    public interface IMultiTenantLocalizationSource : ILocalizationSource
    {
        /// <summary>
        /// Gets a <see cref="LocalizedString"/>.
        /// </summary>
        /// <param name="tenantId">TenantId or null for host.</param>
        /// <param name="name">Localization key name.</param>
        /// <param name="culture">Culture</param>
        string GetString(int? tenantId, string name, CultureInfo culture);

        /// <summary>
        /// Gets a <see cref="LocalizedString"/>.
        /// </summary>
        /// <param name="tenantId">TenantId or null for host.</param>
        /// <param name="name">Localization key name.</param>
        /// <param name="culture">Culture</param>
        /// <param name="tryDefaults">True: fallbacks to default languages if can not find in given culture</param>
        string GetStringOrNull(int? tenantId, string name, CultureInfo culture, bool tryDefaults = true);

        /// <summary>
        /// Gets list of <see cref="LocalizedString"/>.
        /// </summary>
        /// <param name="tenantId">TenantId or null for host.</param>
        /// <param name="names">Localization key name.</param>
        /// <param name="culture">Culture</param>
        List<string> GetStrings(int? tenantId, List<string> names, CultureInfo culture);

        /// <summary>
        /// Gets list of <see cref="LocalizedString"/>.
        /// </summary>
        /// <param name="tenantId">TenantId or null for host.</param>
        /// <param name="names">Localization key name.</param>
        /// <param name="culture">Culture</param>
        /// <param name="tryDefaults">True: fallbacks to default languages if can not find in given culture</param>
        List<string> GetStringsOrNull(int? tenantId, List<string> names, CultureInfo culture, bool tryDefaults = true);
    }
}
