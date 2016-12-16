using System;
using Abp.Dependency;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class ScopedIocResolver_Inject_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Automatically_Release_Resolved_Dependencies_When_Injected_Class_Released()
        {
            //Arrange
            LocalIocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<MyDependency>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<MyMainClass>(DependencyLifeStyle.Transient);

            //Act
            var mainClass = LocalIocManager.Resolve<MyMainClass>();
            var dependency = mainClass.CreateDependency();
            dependency.IsDisposed.ShouldBeFalse();
            LocalIocManager.Release(mainClass);

            //Assert
            dependency.IsDisposed.ShouldBeTrue();
        }

        public class MyDependency : IDisposable
        {
            public bool IsDisposed { get; set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        public class MyMainClass
        {
            private readonly IScopedIocResolver _resolver;

            public MyMainClass(IScopedIocResolver resolver)
            {
                _resolver = resolver;
            }

            public MyDependency CreateDependency()
            {
                return _resolver.Resolve<MyDependency>();
            }
        }
    }
}
