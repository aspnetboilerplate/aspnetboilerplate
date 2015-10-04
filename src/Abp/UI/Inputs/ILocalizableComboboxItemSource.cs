using System.Collections.Generic;

namespace Abp.UI.Inputs
{
    public interface ILocalizableComboboxItemSource
    {
        IEnumerable<ILocalizableComboboxItem> GetItems();
    }
}