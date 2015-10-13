using System;
using System.Collections.Generic;

namespace Abp.UI.Inputs
{
    public class StaticLocalizableComboboxItemSource : ILocalizableComboboxItemSource
    {
        public ILocalizableComboboxItem[] Items { get; set; }

        public StaticLocalizableComboboxItemSource(params ILocalizableComboboxItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Length <= 0)
            {
                throw new ArgumentException("Items can not me empty!");
            }

            Items = items;
        }

        public IEnumerable<ILocalizableComboboxItem> GetItems()
        {
            return Items;
        }
    }
}