using Castle.MicroKernel;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class Circular_Constructor_Dependency_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Fail_Circular_Constructor_Dependency()
        {
            LocalIocManager.Register<MyClass1>();
            LocalIocManager.Register<MyClass2>();
            LocalIocManager.Register<MyClass3>();

            Assert.Throws<CircularDependencyException>(() => LocalIocManager.Resolve<MyClass1>());
        }

        public class MyClass1
        {
            public MyClass1(MyClass2 obj)
            {

            }
        }

        public class MyClass2
        {
            public MyClass2(MyClass3 obj)
            {

            }
        }

        public class MyClass3
        {
            public MyClass3(MyClass1 obj)
            {

            }
        }
    }
}