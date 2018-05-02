using System.Collections.Generic;
using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.Shop
{
    public class Product : Entity, IMultiLingualEntity<ProductTranslation>
    {
        public virtual decimal Price { get; set; }

        public virtual int Stock { get; set; }

        public virtual ICollection<ProductTranslation> Translations { get; set; }
    }
}
