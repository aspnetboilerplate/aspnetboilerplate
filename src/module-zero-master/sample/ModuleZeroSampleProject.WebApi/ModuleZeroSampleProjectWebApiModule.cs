using System.Reflection;
using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi;
using Abp.WebApi.Controllers.Dynamic.Builders;

namespace ModuleZeroSampleProject
{
    [DependsOn(typeof(AbpWebApiModule), typeof(ModuleZeroSampleProjectApplicationModule))]
    public class ModuleZeroSampleProjectWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(ModuleZeroSampleProjectApplicationModule).Assembly, "app")
                .Build();
        }
    }
}
