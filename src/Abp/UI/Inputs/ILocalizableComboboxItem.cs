using Abp.Localization;

namespace Abp.UI.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILocalizableComboboxItem
    {
        string Value { get; set; }

        ILocalizableString DisplayText { get; set; }
    }
}