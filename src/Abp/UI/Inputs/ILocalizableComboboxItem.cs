using Abp.Localization;
using Newtonsoft.Json;

namespace Abp.UI.Inputs
{
    public interface ILocalizableComboboxItem
    {
        string Value { get; set; }

        [JsonConverter(typeof(LocalizableStringToStringJsonConverter))]
        ILocalizableString DisplayText { get; set; }
    }
}