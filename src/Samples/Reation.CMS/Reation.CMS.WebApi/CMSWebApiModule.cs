using System.Reflection;
using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi;
using Abp.WebApi.Controllers.Dynamic.Builders;

namespace Reation.CMS
{
    [DependsOn(typeof(AbpWebApiModule), typeof(CMSApplicationModule))]
    public class CMSWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(CMSApplicationModule).Assembly, "app")
                .Build();
        }
    }
}
