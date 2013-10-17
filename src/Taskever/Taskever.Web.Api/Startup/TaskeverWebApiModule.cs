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

        private static void CreateWebApiProxiesForServices()
        {
            //TODO: must UseConventions be more general insted of controller builder?
            //TODO: must be able to exclude/include all methods option

            BuildApiController
                .For<ITaskService>().WithControllerName("task").WithAreaName("taskever")
                .UseConventions()
                .ForMethod("GetTasksOfUser").WithVerb(HttpVerb.Post)
                .Build(); 

            BuildApiController
                .For<IFriendshipService>()
                .UseConventions()
                .ForMethod("GetMyFriends").WithVerb(HttpVerb.Post)
                .Build();

            BuildApiController
                .For<IUserActivityService>()
                .UseConventions()
                .ForMethod("GetFallowedActivities").WithVerb(HttpVerb.Post)
                .Build();

        }
    }
}