using System.Globalization;

namespace Abp.Localization
{
    /// <summary>
    /// Represents an available language for the application.
    /// </summary>
    public class LanguageInfo
    {
        /// <summary>
        /// Code name of the language.
        /// It should be valid culture code.
        /// Ex: "en-US" for American English, "tr-TR" for Turkey Turkish.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name of the language in it's original language.
        /// Ex: "English" for English, "Türkçe" for Turkish.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// An icon can be set to display on the UI.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Is this the default language?
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Is this the language disabled?
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Is this language Right To Left?
        /// </summary>
        public bool IsRightToLeft
        {
            get
            {
                try
                {
                    return CultureInfo.GetCultureInfo(Name).TextInfo?.IsRightToLeft ?? false;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="LanguageInfo"/> object.
        /// </summary>
        /// <param name="name">
        /// Code name of the language.
        /// It should be valid culture code.
        /// Ex: "en-US" for American English, "tr-TR" for Turkey Turkish.
        /// </param>
        /// <param name="displayName">
        /// Display name of the language in it's original language.
        /// Ex: "English" for English, "Türkçe" for Turkish.
        /// </param>
        /// <param name="icon">An icon can be set to display on the UI</param>
        /// <param name="isDefault">Is this the default language?</param>
        /// <param name="isDisabled">Is this the language disabled?</param>
        public LanguageInfo(string name, string displayName, string icon = null, bool isDefault = false, bool isDisabled = false)
        {
            Name = name;
            DisplayName = displayName;
            Icon = icon;
            IsDefault = isDefault;
            IsDisabled = isDisabled;
        }
    }
}