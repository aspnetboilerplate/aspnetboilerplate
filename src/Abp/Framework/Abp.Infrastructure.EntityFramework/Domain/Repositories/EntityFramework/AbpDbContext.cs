using System.Data.Entity;
using Abp.Dependency;
using Abp.Domain.Entities;

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

        public override int SaveChanges()
        {
            DoSoftDelete();

            return base.SaveChanges();
        }

        private void DoSoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDeleteEntity>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Unchanged;
                    entry.Entity.IsDeleted = true;
                }
            }
        }
    }
}
