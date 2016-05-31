using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class IocManager_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Get_First_Registered_Class_If_Registered_Multiple_Class_For_Same_Interface()
        {
            LocalIocManager.Register<IEmpty, EmptyImplOne>();
            LocalIocManager.Register<IEmpty, EmptyImplTwo>(); //Second registered has no effect!
            LocalIocManager.Resolve<IEmpty>().GetType().ShouldBe(typeof (EmptyImplOne));
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