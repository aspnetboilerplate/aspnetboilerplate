namespace Abp.Localization
{
    public class LocalizedString
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public LocalizedString(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}