using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace MyAbpZeroProject
{
    [DependsOn(typeof(MyAbpZeroProjectCoreModule), typeof(AbpAutoMapperModule))]
    public class MyAbpZeroProjectApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
