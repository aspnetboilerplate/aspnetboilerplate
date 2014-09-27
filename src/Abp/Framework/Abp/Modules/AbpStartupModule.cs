using System.Reflection;
using Abp.Dependency;
using Abp.Dependency.Conventions;
using Abp.Domain.Uow;
using Abp.Events.Bus;

namespace Abp.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public class AbpStartupModule : AbpModule
    {
        public override void PreInitialize()
        {
            base.PreInitialize();

            IocManager.AddConventionalRegisterer(new BasicConventionalRegisterer());
            UnitOfWorkRegistrer.Initialize(IocManager);
        }

        public override void Initialize()
        {
            base.Initialize();

            IocManager.IocContainer.Install(new EventBusInstaller(IocManager));

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }
    }
}