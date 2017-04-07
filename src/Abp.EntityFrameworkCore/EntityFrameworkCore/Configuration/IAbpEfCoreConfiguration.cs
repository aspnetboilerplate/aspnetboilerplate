using System;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Configuration
{
    public interface IAbpEfCoreConfiguration
    {
        void AddDbContext<TDbContext>(Action<AbpDbContextConfiguration<TDbContext>> action)
            where TDbContext : DbContext;
    }
}
