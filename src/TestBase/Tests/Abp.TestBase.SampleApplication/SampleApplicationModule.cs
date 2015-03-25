using System.Reflection;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.EntityFramework;
using Abp.Modules;

namespace Abp.TestBase.SampleApplication
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpAutoMapperModule))]
    public class SampleApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.Selectors.Add(
                new NamedTypeSelector(
                    "AllApplicationServices",
                    type => typeof (IApplicationService).IsAssignableFrom(type)
                    )
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
