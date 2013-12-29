using System.Collections.Generic;

namespace Abp.Localization
{
    /// <summary>
    /// This class is used to manage localization sources. See <see cref="ILocalizationSource"/>.
    /// </summary>
    public interface ILocalizationSourceManager
    {
        /// <summary>
        /// Registers new localization source.
        /// </summary>
        /// <param name="source">Localization source</param>
        void RegisterSource(ILocalizationSource source);

        /// <summary>
        /// Gets a localization source with name.
        /// </summary>
        /// <param name="name">Unique name of the localization source</param>
        /// <returns>The localization source</returns>
        ILocalizationSource GetSource(string name);

        /// <summary>
        /// Gets all registered localization sources.
        /// </summary>
        /// <returns>List of sources</returns>
        IReadOnlyList<ILocalizationSource> GetAllSources();
    }
}