using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Repositories.EntityFramework;
using Abp.Modules;
using Abp.Modules.Core.Startup;
using Abp.Startup;
using Castle.MicroKernel.Registration;
using Taskever.Infrastructure.EntityFramework.Data.Repositories.NHibernate;
using Taskever.Tasks;

namespace Taskever.Infrastructure.EntityFramework.Startup
{
    public class TaskeverDataModule : AbpModule
    {
        public override Type[] GetDependedModules()
        {
            return new[]
                   {
                       typeof (AbpModulesCoreInfrastructureEntityFrameworkModule)
                   };
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            //IocManager.Instance.IocContainer.Register(Component.For<AbpDbContext>().ImplementedBy<TaskeverDbContext>().LifestyleTransient());
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            //AbpDbContext.AddEntityAssembly(Assembly.GetAssembly(typeof(Task)));
        }
    }
}