namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Names of standard caches used in ABP.
    /// </summary>
    public static class AbpCacheNames
    {
        /// <summary>
        /// Application settings cache: AbpApplicationSettingsCache.
        /// </summary>
        public const string ApplicationSettings = "AbpApplicationSettingsCache";

        /// <summary>
        /// Tenant settings cache: AbpTenantSettingsCache.
        /// </summary>
        public const string TenantSettings = "AbpTenantSettingsCache";

        /// <summary>
        /// User settings cache: AbpUserSettingsCache.
        /// </summary>
        public const string UserSettings = "AbpUserSettingsCache";

        /// <summary>
        /// Localization scripts cache: AbpLocalizationScripts.
        /// </summary>
        public const string LocalizationScripts = "AbpLocalizationScripts";
    }
}