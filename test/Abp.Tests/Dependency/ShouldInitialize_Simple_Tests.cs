using Abp.Dependency;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class ShouldInitialize_Simple_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Call_Initialize()
        {
            LocalIocManager.Register<MyService>(DependencyLifeStyle.Transient);
            var myService = LocalIocManager.Resolve<MyService>();
            myService.InitializeCount.ShouldBe(1);
        }

        public class MyService : IShouldInitialize
        {
            public int InitializeCount { get; private set; }

            public void Initialize()
            {
                InitializeCount++;
            }
        }
    }
}
