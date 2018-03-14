using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain
{
    public class ProductTranslation : Entity, IEntityTranslation<Product, int>
    {
        public virtual string Name { get; set; }

        public virtual Product Core { get; set; }

        public virtual int CoreId { get; set; }

        public virtual string Language { get; set; }
    }
}