using System.Linq;
using Abp.Modules;
using Abp.PlugIns;
using Shouldly;
using Xunit;

namespace Abp.Tests.Modules
{
    public class PlugInModuleLoading_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Load_All_Modules()
        {
            //Arrange
            var bootstrapper = AbpBootstrapper.Create<MyStartupModule>(LocalIocManager);

            bootstrapper.PlugInSources.AddTypeList(typeof(MyPlugInModule));

            bootstrapper.Initialize();

            //Act
            var modules = bootstrapper.IocManager.Resolve<IAbpModuleManager>().Modules;

            //Assert
            modules.Count.ShouldBe(6);

            modules.Any(m => m.Type == typeof(AbpKernelModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyStartupModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule1)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule2)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyPlugInModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyPlugInDependedModule)).ShouldBeTrue();

            modules.Any(m => m.Type == typeof(MyNotDependedModule)).ShouldBeFalse();
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

        [DependsOn(typeof(MyPlugInDependedModule))]
        public class MyPlugInModule : AbpModule
        {
            
        }

        public class MyPlugInDependedModule : AbpModule
        {
            
        }
    }
}
