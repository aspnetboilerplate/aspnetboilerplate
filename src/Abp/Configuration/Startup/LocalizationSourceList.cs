using System.Collections.Generic;
using Abp.Localization.Sources;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// A specialized list to store <see cref="ILocalizationSource"/> object.
    /// </summary>
    public class LocalizationSourceList : List<ILocalizationSource>, ILocalizationSourceList
    {
        
    }
}