using System.Collections.Generic;

namespace Abp.Localization
{
    public interface ILocalizationManager
    {
        void RegisterSource(ILocalizationSource source);

        ILocalizationSource GetSource(string sourceName);

        List<ILocalizationSource> GetAllSources();
    }
}