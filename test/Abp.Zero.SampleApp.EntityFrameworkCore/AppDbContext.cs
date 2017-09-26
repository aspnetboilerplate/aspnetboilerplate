using Abp.Zero.EntityFrameworkCore;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Users;
using Microsoft.EntityFrameworkCore;

namespace Abp.Zero.SampleApp.EntityFrameworkCore
{
    public class AppDbContext : AbpZeroDbContext<Tenant, Role, User, AppDbContext>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {

        }
    }
}