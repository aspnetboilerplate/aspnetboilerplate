using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace Abp.EntityFramework
{
    public interface IDbContextEntityFinder
    {
        IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType);
    }
}