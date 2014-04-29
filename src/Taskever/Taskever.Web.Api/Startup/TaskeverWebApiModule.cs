using Abp.Modules;
using Abp.Startup;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Taskever.Activities;
using Taskever.Friendships;
using Taskever.Tasks;
using Taskever.Users;
using Taskever.Web.Dependency.Installers;

namespace Taskever.Web.Startup
{
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

            DynamicApiControllerBuilder
                .For<ITaskeverUserAppService>("taskever/user")
                .Build();

            DynamicApiControllerBuilder
                .For<ITaskAppService>("taskever/task")
                .Build(); 

            DynamicApiControllerBuilder
                .For<IFriendshipAppService>("taskever/friendship")
                .Build();

            DynamicApiControllerBuilder
                .For<IUserActivityAppService>("taskever/userActivity")
                .Build();
        }
    }
}