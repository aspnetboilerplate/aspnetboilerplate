using System.ComponentModel;

namespace Abp.Localization
{
    public class AbpDisplayNameAttribute : DisplayNameAttribute
    {
        public override string DisplayName => LocalizationHelper.GetString(SourceName, Key);

        public string SourceName { get; set; }
        public string Key { get; set; }

        public AbpDisplayNameAttribute(string sourceName, string key)
        {
            SourceName = sourceName;
            Key = key;
        }
    }
}
