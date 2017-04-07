using System;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Configuration
{
    public class AbpDbContextConfigurerAction<TDbContext> : IAbpDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        public Action<AbpDbContextConfiguration<TDbContext>> Action { get; set; }

        public AbpDbContextConfigurerAction(Action<AbpDbContextConfiguration<TDbContext>> action)
        {
            Action = action;
        }

        public void Configure(AbpDbContextConfiguration<TDbContext> configuration)
        {
            Action(configuration);
        }
    }
}