using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Reflection;
using Abp.EntityFramework.Dependency;
using Abp.EntityFramework.Repositories;
using Abp.EntityFramework.SoftDeleting;
using Abp.Modules;
using Abp.Reflection;

namespace Abp.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in NHibernate.
    /// </summary>
    public class AbpEntityFrameworkModule : AbpModule
    {
        public IAssemblyFinder AssemblyFinder { private get; set; }

        public AbpEntityFrameworkModule()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
        }

        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new EntityFrameworkConventionalRegisterer());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            DbInterception.Add(new SoftDeleteInterceptor());
            RegisterGenericRepositories();
        }

        private void RegisterGenericRepositories()
        {
            var dbContextTypes = (
                from assembly in AssemblyFinder.GetAllAssemblies()
                from type in assembly.GetTypes()
                where type.IsPublic && !type.IsAbstract && type.IsClass && typeof(AbpDbContext).IsAssignableFrom(type)
                select type
                ).ToList();

            foreach (var dbContextType in dbContextTypes)
            {
                EntityFrameworkGenericRepositoryRegistrar.RegisterDbContext(dbContextType, IocManager);
            }
        }
    }
}
