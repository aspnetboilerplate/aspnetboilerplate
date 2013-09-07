using System.Reflection;
using Abp.Data;
using Abp.Modules;
using Abp.Web.Controllers.Dynamic;
using Castle.Windsor.Installer;
using Taskever.Entities.NHibernate.Mappings;
using Taskever.Services;
using Taskever.Web.Dependency;

namespace Taskever.Web.App_Start
{
    [AbpModule("Taskever", Dependencies = new[] { "Abp.Modules.Core" })]
    public class TaskeverModule : AbpModule
    {
        public override void PreInitialize(AbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            GetModule<AbpDataModule>().AddMapping(m => m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(TaskMap))));
        }

        public override void Initialize(AbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(FromAssembly.This());
            AutoMappingManager.Map();
            DynamicControllerGenerator.GenerateFor<ITaskService>(); //TODO: where to write?
        }
    }
}