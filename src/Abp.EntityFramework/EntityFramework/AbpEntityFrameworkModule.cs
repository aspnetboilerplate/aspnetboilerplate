using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.EntityFramework.Dependency;
using Abp.EntityFramework.Repositories;
using Abp.EntityFramework.SoftDeleting;
using Abp.EntityFramework.Uow;
using Abp.Modules;
using Abp.Reflection;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;

namespace Abp.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in EntityFramework.
    /// </summary>
    public class AbpEntityFrameworkModule : AbpModule
    {
        public ILogger Logger { get; set; }

        private readonly ITypeFinder _typeFinder;

        public AbpEntityFrameworkModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
            Logger = NullLogger.Instance;
        }

        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new EntityFrameworkConventionalRegisterer());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(
                Component.For(typeof (IDbContextProvider<>))
                    .ImplementedBy(typeof (UnitOfWorkDbContextProvider<>))
                    .LifestyleTransient()
                );
            
            RegisterGenericRepositories();

            DbInterception.Add(new SoftDeleteInterceptor());
        }

        private void RegisterGenericRepositories()
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

            foreach (var dbContextType in dbContextTypes)
            {
                EntityFrameworkGenericRepositoryRegistrar.RegisterForDbContext(dbContextType, IocManager);
            }
        }
    }
}
