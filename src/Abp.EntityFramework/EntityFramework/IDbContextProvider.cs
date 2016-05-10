using System;
using System.Data.Entity;
using Abp.MultiTenancy;

namespace Abp.EntityFramework
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        [Obsolete("Use GetDbContext() method instead")]
        TDbContext DbContext { get; }

        TDbContext GetDbContext();

        TDbContext GetDbContext(MultiTenancySides? multiTenancySide);
    }
}