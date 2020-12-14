using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Abp.MultiTenancy;

namespace Abp.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();

        TDbContext GetDbContext(MultiTenancySides? multiTenancySide );
        
        Task<TDbContext> GetDbContextAsync();

        Task<TDbContext> GetDbContextAsync(MultiTenancySides? multiTenancySide );
    }
}
