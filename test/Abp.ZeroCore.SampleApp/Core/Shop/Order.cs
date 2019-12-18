using System.Collections.Generic;
using Abp.Domain.Entities;
using Castle.Components.DictionaryAdapter;

namespace Abp.ZeroCore.SampleApp.Core.Shop
{
    public class Order: Entity, IMultiLingualEntity<OrderTranslation>
    {
        public virtual decimal Price { get; set; }

        public ICollection<OrderTranslation> Translations { get; set; }

        public List<Product> Products { get; set; }

        public Order()
        {
            Products = new List<Product>();
        }
    }
}