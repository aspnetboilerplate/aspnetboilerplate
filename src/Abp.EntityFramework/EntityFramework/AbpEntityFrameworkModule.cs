using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFramework.Repositories;
using Abp.EntityFramework.Uow;
using Abp.Modules;
using Abp.Reflection;
using Castle.MicroKernel.Registration;

namespace Abp.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in EntityFramework.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpEntityFrameworkModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;

        public AbpEntityFrameworkModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            Configuration.ReplaceService<IUnitOfWorkFilterExecuter>(() =>
            {
                IocManager.IocContainer.Register(
                    Component
                    .For<IUnitOfWorkFilterExecuter, IEfUnitOfWorkFilterExecuter>()
                    .ImplementedBy<EfDynamicFiltersUnitOfWorkFilterExecuter>()
                    .LifestyleTransient()
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(
                Component.For(typeof(IDbContextProvider<>))
                    .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                    .LifestyleTransient()
                );

            RegisterGenericRepositoriesAndMatchDbContexes();
        }

        private void RegisterGenericRepositoriesAndMatchDbContexes()
        {
            var dbContextTypes =
                _typeFinder.Find(type =>
                    type.IsPublic &&
                    !type.IsAbstract &&
                    type.IsClass &&
                    typeof(AbpDbContext).IsAssignableFrom(type)
                    );

            if (dbContextTypes.IsNullOrEmpty())
            {
                Logger.Warn("No class found derived from AbpDbContext.");
                return;
            }

            using (var repositoryRegistrar = IocManager.ResolveAsDisposable<IEntityFrameworkGenericRepositoryRegistrar>())
            {
                foreach (var dbContextType in dbContextTypes)
                {
                    Logger.Debug("Registering DbContext: " + dbContextType.AssemblyQualifiedName);
                    repositoryRegistrar.Object.RegisterForDbContext(dbContextType, IocManager);
                }
            }

            using (var dbContextMatcher = IocManager.ResolveAsDisposable<IDbContextTypeMatcher>())
            {
                dbContextMatcher.Object.Populate(dbContextTypes);
            }
        }
    }
}
