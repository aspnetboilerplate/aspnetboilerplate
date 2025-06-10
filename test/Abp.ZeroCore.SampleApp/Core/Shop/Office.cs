using System.Collections.Generic;
using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.Shop;

public class Office : Entity, IMultiLingualEntity<OfficeTranslation>
{
    public int Capacity { get; set; }

    public ICollection<OfficeTranslation> Translations { get; set; }
}