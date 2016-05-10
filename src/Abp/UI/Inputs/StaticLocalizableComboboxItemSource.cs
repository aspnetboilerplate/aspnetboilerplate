using System;
using System.Collections.Generic;

namespace Abp.UI.Inputs
{
    public class StaticLocalizableComboboxItemSource : ILocalizableComboboxItemSource
    {
        public StaticLocalizableComboboxItemSource(params ILocalizableComboboxItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Length <= 0)
            {
                throw new ArgumentException("Items can not be empty!");
            }

            Items = items;
        }

        public ICollection<ILocalizableComboboxItem> Items { get; }
    }
}