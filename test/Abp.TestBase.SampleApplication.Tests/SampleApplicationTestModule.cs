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
#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once the UseStaticMapper property is removed
			Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;
#pragma warning restore CS0618 // Type or member is obsolete, this line will be removed once the UseStaticMapper property is removed

            Configuration.UnitOfWork.ConventionalUowSelectors.Add(type => type == typeof(MyCustomUowClass));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
