using System.Linq;
using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class IocManager_Override_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Not_Override_As_Default()
        {
            //Arrange
            LocalIocManager.Register<IMyService, MyImpl1>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<IMyService, MyImpl2>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<IMyService, MyImpl3>(DependencyLifeStyle.Transient);

            //Act
            var service = LocalIocManager.Resolve<IMyService>();
            var allServices = LocalIocManager.IocContainer.ResolveAll<IMyService>();

            //Assert
            service.ShouldBeOfType<MyImpl1>();
            allServices.Length.ShouldBe(3);
            allServices.Any(s => s.GetType() == typeof(MyImpl1)).ShouldBeTrue();
            allServices.Any(s => s.GetType() == typeof(MyImpl2)).ShouldBeTrue();
            allServices.Any(s => s.GetType() == typeof(MyImpl3)).ShouldBeTrue();
        }

        [Fact]
        public void Should_Override_When_Using_IsDefault()
        {
            //Arrange
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl1>().LifestyleTransient());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl2>().LifestyleTransient().IsDefault());

            //Act
            var service = LocalIocManager.Resolve<IMyService>();
            var allServices = LocalIocManager.IocContainer.ResolveAll<IMyService>();

            //Assert
            service.ShouldBeOfType<MyImpl2>();
            allServices.Length.ShouldBe(2);
            allServices.Any(s => s.GetType() == typeof(MyImpl1)).ShouldBeTrue();
            allServices.Any(s => s.GetType() == typeof(MyImpl2)).ShouldBeTrue();
        }

        [Fact]
        public void Should_Override_When_Using_IsDefault_Twice()
        {
            //Arrange
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl1>().LifestyleTransient());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl2>().LifestyleTransient().IsDefault());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl3>().LifestyleTransient().IsDefault());

            //Act
            var service = LocalIocManager.Resolve<IMyService>();
            var allServices = LocalIocManager.IocContainer.ResolveAll<IMyService>();

            //Assert
            service.ShouldBeOfType<MyImpl3>();
            allServices.Length.ShouldBe(3);
            allServices.Any(s => s.GetType() == typeof(MyImpl1)).ShouldBeTrue();
            allServices.Any(s => s.GetType() == typeof(MyImpl2)).ShouldBeTrue();
            allServices.Any(s => s.GetType() == typeof(MyImpl3)).ShouldBeTrue();
        }

        [Fact]
        public void Should_Get_Default_Service()
        {
            //Arrange
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl1>().LifestyleTransient());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl2>().LifestyleTransient().IsDefault());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl3>().LifestyleTransient());

            //Act
            var service = LocalIocManager.Resolve<IMyService>();
            var allServices = LocalIocManager.IocContainer.ResolveAll<IMyService>();

            //Assert
            service.ShouldBeOfType<MyImpl2>();
            allServices.Length.ShouldBe(3);
            allServices.Any(s => s.GetType() == typeof(MyImpl1)).ShouldBeTrue();
            allServices.Any(s => s.GetType() == typeof(MyImpl2)).ShouldBeTrue();
            allServices.Any(s => s.GetType() == typeof(MyImpl3)).ShouldBeTrue();
        }

        public class MyImpl1 : IMyService
        {
            
        }

        public class MyImpl2 : IMyService
        {

        }

        public class MyImpl3 : IMyService
        {

        }

        public interface IMyService
        {
        }
    }
}
