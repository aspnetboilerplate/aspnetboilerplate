using System.Reflection;
using Adorable.AutoMapper;
using Adorable.EntityFramework;
using Adorable.Modules;
using Adorable.TestBase.SampleApplication.ContacLists;

namespace Adorable.TestBase.SampleApplication
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
