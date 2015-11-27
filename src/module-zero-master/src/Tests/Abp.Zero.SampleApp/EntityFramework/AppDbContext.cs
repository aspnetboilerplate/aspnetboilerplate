using System.Data.Common;
using Abp.Zero.EntityFramework;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Users;

namespace Abp.Zero.SampleApp.EntityFramework
{
    public class AppDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        public AppDbContext(DbConnection connection)
            : base(connection, true)
        {

        }
    }
}