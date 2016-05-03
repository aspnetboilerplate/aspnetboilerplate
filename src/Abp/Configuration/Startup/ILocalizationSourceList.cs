using Abp.Localization.Sources;
using System.Collections.Generic;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Defines a specialized list to store <see cref="ILocalizationSource"/> object.
    /// </summary>
    public interface ILocalizationSourceList : IList<ILocalizationSource>
    {
        /// <summary>
        /// Extensions for dictionay based localization sources.
        /// </summary>
        IList<LocalizationSourceExtensionInfo> Extensions { get; }
    }
}