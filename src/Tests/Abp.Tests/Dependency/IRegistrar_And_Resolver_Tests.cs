using Abp.Dependency;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class Registrar_And_Resolver_Tests : TestBaseWithLocalIocManager
    {
        private readonly IIocRegistrar _registrar;
        private readonly IIocResolver _resolver;

        public Registrar_And_Resolver_Tests()
        {
            _registrar = LocalIocManager.Resolve<IIocRegistrar>();
            _resolver = LocalIocManager.Resolve<IIocResolver>();
        }

        [Fact]
        public void Should_Resolve_Self_Registered_Types()
        {
            _registrar.Register<MyClass>();
            _resolver.Resolve<MyClass>();
        }

        [Fact]
        public void Should_Resolve_Registered_By_Interface_Types()
        {
            _registrar.Register<IMyInterface, MyClass>();
            _resolver.Resolve<IMyInterface>();

            try
            {
                _resolver.Resolve<MyClass>();
                Assert.False(true, "Should not resolve by class that is registered by interface");
            }
            catch { }
        }

        [Fact]
        public void Should_Get_Different_Objects_For_Transients()
        {
            _registrar.Register<MyClass>(DependencyLifeStyle.Transient);
            
            var obj1 = _resolver.Resolve<MyClass>();
            var obj2 = _resolver.Resolve<MyClass>();

            obj1.ShouldNotBeSameAs(obj2);
        }
        [Fact]
        public void Should_Get_Same_Object_For_Singleton()
        {
            _registrar.Register<MyClass>(DependencyLifeStyle.Singleton);

            var obj1 = _resolver.Resolve<MyClass>();
            var obj2 = _resolver.Resolve<MyClass>();

            obj1.ShouldBeSameAs(obj2);
        }


        public class MyClass : IMyInterface
        {
            
        }

        public interface IMyInterface
        {

        }
    }
}