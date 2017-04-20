using System;
using System.Reflection;

using Abp.Configuration.Startup;
using Abp.Dapper.Repositories;
using Abp.Dependency;
using Abp.EntityFramework;
using Abp.EntityFramework.Uow;
using Abp.Modules;
using Abp.Reflection;

namespace Abp.Dapper
{
    [DependsOn(
        typeof(AbpEntityFrameworkModule),
        typeof(AbpKernelModule)
    )]
    public class AbpDapperModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;

        public AbpDapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            Configuration.ReplaceService<IEfTransactionStrategy, DbContextEfTransactionStrategy>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            RegisterDapperGenericRepositories();
        }

        private void RegisterDapperGenericRepositories()
        {
            Type[] dbContextTypes =
                _typeFinder.Find(type =>
                    type.IsPublic &&
                    !type.IsAbstract &&
                    type.IsClass &&
                    typeof(AbpDbContext).IsAssignableFrom(type)
                );

            using (IDisposableDependencyObjectWrapper<IDapperGenericRepositoryRegistrar> repositoryRegistrar = IocManager.ResolveAsDisposable<IDapperGenericRepositoryRegistrar>())
            {
                foreach (Type dbContextType in dbContextTypes)
                {
                    Logger.Debug("Registering DbContext: " + dbContextType.AssemblyQualifiedName);
                    repositoryRegistrar.Object.RegisterForDbContext(dbContextType, IocManager);
                }
            }
        }
    }
}
