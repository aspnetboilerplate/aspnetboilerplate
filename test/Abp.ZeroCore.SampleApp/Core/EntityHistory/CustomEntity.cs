using System;
using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory;

public class CustomEntity : Entity
{
    public string Name { get; set; }
}

public class CustomEntityWithGuidId : Entity<Guid>
{
    public string Name { get; set; }
}