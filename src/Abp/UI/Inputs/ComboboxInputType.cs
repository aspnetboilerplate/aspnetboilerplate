using System;
using Abp.Runtime.Validation;

namespace Abp.UI.Inputs
{
    /// <summary>
    /// Combobox value UI type.
    /// </summary>
    [Serializable]
    [InputType("COMBOBOX")]
    public class ComboboxInputType : InputTypeBase
    {
        public ILocalizableComboboxItemSource Source { get; set; }

        public ComboboxInputType()
        {

        }

        public ComboboxInputType(ILocalizableComboboxItemSource source)
        {
            Source = source;
        }

        public ComboboxInputType(ILocalizableComboboxItemSource source, IValueValidator validator)
            : base(validator)
        {
            Source = source;
        }
    }
}