using System.Linq;
using System.Reflection;
using Abp.Modules;
using Abp.Reflection;
using Abp.Reflection.Extensions;
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
            var bootstrapper = AbpBootstrapper.Create<MyStartupModule>(options =>
            {
                options.IocManager = LocalIocManager;
            });

            bootstrapper.Initialize();

            //Act
            var assemblies = bootstrapper.IocManager.Resolve<AbpAssemblyFinder>().GetAllAssemblies();

            //Assert
            assemblies.Count.ShouldBe(3);

            assemblies.Any(a => a == typeof(MyStartupModule).GetAssembly()).ShouldBeTrue();
            assemblies.Any(a => a == typeof(AbpKernelModule).GetAssembly()).ShouldBeTrue();
            assemblies.Any(a => a == typeof(FactAttribute).GetAssembly()).ShouldBeTrue();
        }

        public class MyStartupModule : AbpModule
        {
            public override Assembly[] GetAdditionalAssemblies()
            {
                return new[] {typeof(FactAttribute).GetAssembly()};
            }
        }
    }
}