namespace Abp.Configuration
{
    internal class SettingValue : ISettingValue
    {
        public string Name { get; private set; }

        public string Value { get; private set; }

        public SettingValue(string name, string value)
        {
            Value = value;
            Name = name;
        }
    }
}