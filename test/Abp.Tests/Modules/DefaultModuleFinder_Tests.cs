using System.Linq;
using Abp.Modules;
using Shouldly;
using Xunit;

namespace Abp.Tests.Modules
{
    public class DefaultModuleFinder_Tests
    {
        [Fact]
        public void Should_Get_Load_Depended_Modules()
        {
            //Arrange
            var bootstrapper = AbpBootstrapper.Create<MyStartupModule>();
            bootstrapper.Initialize();

            //Act
            var modules = bootstrapper.IocManager.Resolve<DefaultModuleFinder>().FindAll();

            //Assert
            modules.Count.ShouldBe(4);

            modules.Any(m => m == typeof(AbpKernelModule)).ShouldBeTrue();
            modules.Any(m => m == typeof(MyStartupModule)).ShouldBeTrue();
            modules.Any(m => m == typeof(MyModule1)).ShouldBeTrue();
            modules.Any(m => m == typeof(MyModule2)).ShouldBeTrue();

            modules.Any(m => m == typeof(MyNotDependedModule)).ShouldBeFalse();
        }

        [DependsOn(typeof(MyModule1), typeof(MyModule2))]
        public class MyStartupModule: AbpModule
        {

        }

        public class MyModule1 : AbpModule
        {
            
        }

        public class MyModule2 : AbpModule
        {

        }
        
        public class MyNotDependedModule : AbpModule
        {

        }
    }
}
