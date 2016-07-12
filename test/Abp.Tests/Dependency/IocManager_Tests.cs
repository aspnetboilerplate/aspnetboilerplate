using System.Linq;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class IocManager_Tests : TestBaseWithLocalIocManager
    {
        public IocManager_Tests()
        {
            LocalIocManager.Register<IEmpty, EmptyImplOne>();
            LocalIocManager.Register<IEmpty, EmptyImplTwo>();
        }

        [Fact]
        public void Should_Get_First_Registered_Class_If_Registered_Multiple_Class_For_Same_Interface()
        {
            LocalIocManager.Resolve<IEmpty>().GetType().ShouldBe(typeof (EmptyImplOne));
        }

        [Fact]
        public void ResolveAll_Test()
        {
            var instances = LocalIocManager.ResolveAll<IEmpty>();
            instances.Length.ShouldBe(2);
            instances.Any(i => i.GetType() == typeof(EmptyImplOne)).ShouldBeTrue();
            instances.Any(i => i.GetType() == typeof(EmptyImplTwo)).ShouldBeTrue();
        }

        public interface IEmpty
        {
            
        }

        public class EmptyImplOne : IEmpty
        {
            
        }

        public class EmptyImplTwo : IEmpty
        {

        }
    }
}