using System.Reflection;
using Abp.AutoMapper;
using Abp.EntityFramework;
using Abp.Modules;

namespace Abp.TestBase.SampleApplication
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpAutoMapperModule))]
    public class SampleApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
