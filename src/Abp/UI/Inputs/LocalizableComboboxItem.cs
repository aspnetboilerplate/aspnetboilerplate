using System;
using Abp.Localization;

namespace Abp.UI.Inputs
{
    [Serializable]
    public class LocalizableComboboxItem : ILocalizableComboboxItem
    {
        public string Value { get; set; }

        public ILocalizableString DisplayText { get; set; }

        public LocalizableComboboxItem()
        {
            
        }

        public LocalizableComboboxItem(string value, ILocalizableString displayText)
        {
            Value = value;
            DisplayText = displayText;
        }
    }
}