using System.Collections.Generic;
using Abp.Localization.Sources;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Defines a specialized list to store <see cref="ILocalizationSource"/> object.
    /// </summary>
    public interface ILocalizationSourceList : IList<ILocalizationSource>
    {
    }
}