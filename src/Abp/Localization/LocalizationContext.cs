using Abp.Dependency;

namespace Abp.Localization
{
    /// <summary>
    /// Implements <see cref="ILocalizationContext"/>.
    /// </summary>
    public class LocalizationContext : ILocalizationContext, ISingletonDependency
    {
        public ILocalizationManager LocalizationManager { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationContext"/> class.
        /// </summary>
        /// <param name="localizationManager">The localization manager.</param>
        public LocalizationContext(ILocalizationManager localizationManager)
        {
            LocalizationManager = localizationManager;
        }
    }
}