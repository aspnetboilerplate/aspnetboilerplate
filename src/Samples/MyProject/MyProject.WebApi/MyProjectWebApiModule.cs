using System.Reflection;
using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi;
using Abp.WebApi.Controllers.Dynamic.Builders;
using MyProject.News;
using MyProject.Tasks;

namespace MyProject
{
    [DependsOn(typeof(AbpWebApiModule), typeof(MyProjectApplicationModule))]
    public class MyProjectWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(MyProjectApplicationModule).Assembly, "app")
                .Build();

            //DynamicApiControllerBuilder
            //   .ForAll<ITaskAppService>(typeof(MyProjectApplicationModule).Assembly, "app")
            //   .Build();

        }
    }
}
