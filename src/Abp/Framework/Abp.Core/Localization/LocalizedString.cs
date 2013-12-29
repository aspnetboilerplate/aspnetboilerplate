namespace Abp.Localization
{
    /// <summary>
    /// Represents a localized string.
    /// </summary>
    public class LocalizedString
    {
        /// <summary>
        /// Unique Name of the string.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Value for the <see cref="Name"/>.
        /// </summary>
        public string Value { get; private set; }

        /// <param name="name">Unique Name of the string</param>
        /// <param name="value">Value for the <see cref="name"/></param>
        public LocalizedString(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}