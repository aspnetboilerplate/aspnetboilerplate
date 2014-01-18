using Abp.Modules;
using Abp.Startup;
using Abp.WebApi.Controllers;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Taskever.Application.Services;
using Taskever.Web.Dependency.Installers;

namespace Taskever.Web.Startup
{
    [AbpModule("Taskever.Web.Api", Dependencies = new[] { "Abp.Web.Api" })]
    public class TaskeverWebApiModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new TaskeverWebInstaller());
            CreateWebApiProxiesForServices();
        }

        private static void CreateWebApiProxiesForServices()
        {
            //TODO: must be able to exclude/include all methods option

            DyanmicApiControllerBuilder
                .For<ITaskeverUserService>("taskever/user")
                .Build();

            DyanmicApiControllerBuilder
                .For<ITaskService>("taskever/task")
                .Build(); 

            DyanmicApiControllerBuilder
                .For<IFriendshipService>("taskever/friendship")
                .Build();

            DyanmicApiControllerBuilder
                .For<IUserActivityService>("taskever/userActivity")
                .Build();

        }
    }
}