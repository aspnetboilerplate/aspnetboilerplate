using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.TestBase.SampleApplication.Tests.Uow;

namespace Abp.TestBase.SampleApplication.Tests
{
    [DependsOn(typeof(SampleApplicationModule), typeof(AbpTestBaseModule), typeof(AbpAutoMapperModule))]
    public class SampleApplicationTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

            Configuration.UnitOfWork.ConventionalUowSelectors.Add(type => type == typeof(MyCustomUowClass));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
