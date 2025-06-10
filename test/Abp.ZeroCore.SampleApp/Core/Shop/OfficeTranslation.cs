using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.Shop;

public class OfficeTranslation : Entity<long>, IEntityTranslation<Office>
{
    public virtual string Name { get; set; }

    public string Language { get; set; }

    public Office Core { get; set; }

    public int CoreId { get; set; }
}