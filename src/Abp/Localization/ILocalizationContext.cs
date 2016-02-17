namespace Abp.Localization
{
    /// <summary>
    /// Localization context.
    /// </summary>
    public interface ILocalizationContext
    {
        /// <summary>
        /// Gets the localization manager.
        /// </summary>
        ILocalizationManager LocalizationManager { get; }
    }
}