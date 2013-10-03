using Abp.Modules;
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

        private void CreateWebApiProxiesForServices()
        {
            //TODO: must UseConventions be more general insted of controller builder?

            BuildApiController
                .For<ITaskService>().WithControllerName("task") 
                .UseConventions()
                .ForMethod("GetTasksOfUser").WithVerb(HttpVerb.Post)
                .Build(); 

            BuildApiController
                .For<IFriendshipService>()
                .UseConventions()
                .ForMethod("GetMyFriends").WithVerb(HttpVerb.Post)
                .Build();
        }
    }
}