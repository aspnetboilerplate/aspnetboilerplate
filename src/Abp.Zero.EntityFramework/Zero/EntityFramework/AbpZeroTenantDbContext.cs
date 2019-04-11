using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;

namespace Abp.Zero.EntityFramework
{
    [MultiTenancySide(MultiTenancySides.Tenant)]
    public abstract class AbpZeroTenantDbContext<TRole, TUser> : AbpZeroCommonDbContext<TRole, TUser>
        where TRole : AbpRole<TUser>
        where TUser : AbpUser<TUser>
    {
        /// <summary>
        /// Default constructor.
        /// Do not directly instantiate this class. Instead, use dependency injection!
        /// </summary>
        protected AbpZeroTenantDbContext()
        {

        }

        /// <summary>
        /// Constructor with connection string parameter.
        /// </summary>
        /// <param name="nameOrConnectionString">Connection string or a name in connection strings in configuration file</param>
        protected AbpZeroTenantDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        protected AbpZeroTenantDbContext(DbCompiledModel model)
            : base(model)
        {

        }

        /// <summary>
        /// This constructor can be used for unit tests.
        /// </summary>
        protected AbpZeroTenantDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }

        protected AbpZeroTenantDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        protected AbpZeroTenantDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        protected AbpZeroTenantDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }
    }
}