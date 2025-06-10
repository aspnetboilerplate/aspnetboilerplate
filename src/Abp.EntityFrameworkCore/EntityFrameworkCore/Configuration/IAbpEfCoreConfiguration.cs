using System;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Configuration;

public interface IAbpEfCoreConfiguration
{
    public bool UseAbpQueryCompiler { get; set; }

    void AddDbContext<TDbContext>(Action<AbpDbContextConfiguration<TDbContext>> action)
        where TDbContext : DbContext;
}

public class NullAbpEfCoreConfiguration : IAbpEfCoreConfiguration
{
    /// <summary>
    /// Gets single instance of <see cref="NullAbpEfCoreConfiguration"/> class.
    /// </summary>
    public static NullAbpEfCoreConfiguration Instance { get; } = new NullAbpEfCoreConfiguration();

    public bool UseAbpQueryCompiler { get; set; }

    public void AddDbContext<TDbContext>(Action<AbpDbContextConfiguration<TDbContext>> action) where TDbContext : DbContext
    {

    }
}