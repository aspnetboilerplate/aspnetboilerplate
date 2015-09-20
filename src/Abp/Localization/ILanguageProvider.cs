using System.Collections.Generic;

namespace Abp.Localization
{
    public interface ILanguageProvider
    {
        IReadOnlyList<LanguageInfo> GetLanguages();
    }
}