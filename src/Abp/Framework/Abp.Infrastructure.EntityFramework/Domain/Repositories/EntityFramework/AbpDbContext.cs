using System.Data.Entity;
using Abp.Dependency;

namespace Abp.Domain.Repositories.EntityFramework
{
    public abstract class AbpDbContext : DbContext, ITransientDependency
    {
        protected AbpDbContext()
        {

        }

        protected AbpDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
    }
}
