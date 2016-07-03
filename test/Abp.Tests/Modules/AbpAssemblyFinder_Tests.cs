using System.Linq;
using System.Reflection;
using Abp.Modules;
using Abp.Reflection;
using Shouldly;
using Xunit;

namespace Abp.Tests.Modules
{
    public class AbpAssemblyFinder_Tests: TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Get_Module_And_Additional_Assemblies()
        {
            //Arrange
            var bootstrapper = AbpBootstrapper.Create<MyStartupModule>(LocalIocManager);
            bootstrapper.Initialize();

            //Act
            var assemblies = bootstrapper.IocManager.Resolve<AbpAssemblyFinder>().GetAllAssemblies();

            //Assert
            assemblies.Count.ShouldBe(3);

            assemblies.Any(a => a == typeof(MyStartupModule).Assembly).ShouldBeTrue();
            assemblies.Any(a => a == typeof(AbpKernelModule).Assembly).ShouldBeTrue();
            assemblies.Any(a => a == typeof(FactAttribute).Assembly).ShouldBeTrue();
        }

        public class MyStartupModule : AbpModule
        {
            public override Assembly[] GetAdditionalAssemblies()
            {
                return new[] {typeof(FactAttribute).Assembly};
            }
        }
    }
}