using System.Reflection;
using Abp.AutoMapper;
using Abp.EntityFramework;
using Abp.Modules;
using Abp.TestBase.SampleApplication.ContacLists;

namespace Abp.TestBase.SampleApplication
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpAutoMapperModule))]
    public class SampleApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Features.Providers.Add<SampleFeatureProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
