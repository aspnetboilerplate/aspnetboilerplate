using Abp.Dependency;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class IocManager_Self_Register_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Self_Register_With_All_Interfaces()
        {
            var registrar = LocalIocManager.Resolve<IIocRegistrar>();
            var resolver = LocalIocManager.Resolve<IIocResolver>();
            var managerByInterface = LocalIocManager.Resolve<IIocManager>();
            var managerByClass = LocalIocManager.Resolve<IocManager>();

            managerByClass.ShouldBeSameAs(registrar);
            managerByClass.ShouldBeSameAs(resolver);
            managerByClass.ShouldBeSameAs(managerByInterface);
        }
    }
}