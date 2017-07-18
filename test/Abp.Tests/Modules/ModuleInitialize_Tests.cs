using System.Linq;
using Abp.Modules;
using Abp.Web;
using Shouldly;
using Xunit;

namespace Abp.Tests.Modules
{
 public  class ModuleInitialize_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void AbpWebCommonModule_ShouldBe_Initizalize()
        {
            //Arrange
            var bootstrapper = AbpBootstrapper.Create<AbpWebCommonModule>(LocalIocManager);
            bootstrapper.Initialize();

            //Act
            var modules = bootstrapper.IocManager.Resolve<IAbpModuleManager>().Modules;

            //Assert
            modules.Any(m => m.Type == typeof(AbpWebCommonModule)).ShouldBeTrue();
        }
    }
}
