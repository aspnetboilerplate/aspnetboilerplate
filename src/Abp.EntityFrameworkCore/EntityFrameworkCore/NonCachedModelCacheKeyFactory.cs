using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Abp.EntityFrameworkCore;

/// <summary>
/// Avoid unit test caching the configure of the entity.
/// OnModelCreating will be executed multiple times
/// </summary>
public class NonCachedModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context)
    {
        return Guid.NewGuid();
    }

    public virtual object Create(DbContext context, bool designTime)
    {
        return Guid.NewGuid();
    }
}
