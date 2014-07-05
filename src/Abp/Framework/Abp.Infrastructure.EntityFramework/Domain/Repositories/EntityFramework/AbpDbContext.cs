using System.Data.Entity;
using Abp.Dependency;

namespace Abp.Domain.Repositories.EntityFramework
{
    public abstract class AbpDbContext : DbContext, ITransientDependency
    {
        //private static readonly List<Assembly> _entityAssemblies = new List<Assembly>();

        protected AbpDbContext() //TODO@Halil: Remove empty dbContext?
        {

        }

        protected AbpDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
    }
}
