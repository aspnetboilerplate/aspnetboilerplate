using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class Unicode_Naming_Tests
    {
        [Fact]
        public void Castle_Should_Support_Unicode_Class_Names()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<Iお知らせAppService>().ImplementedBy<お知らせAppService>().LifestyleTransient()
            );

            container.Resolve<Iお知らせAppService>().ShouldBeOfType<お知らせAppService>();
        }

        //[Fact] This test is failing because Castle Windsor does not support it
        public void Castle_Should_Register_Unicode_Names_In_Conventions()
        {
            var container = new WindsorContainer();

            container.Register(
                Classes
                    .FromThisAssembly()
                    .Where(c => c == typeof(お知らせAppService))
                    .WithServiceDefaultInterfaces()
                    .WithServiceSelf()
                    .LifestyleTransient()
            );

            container.Resolve<お知らせAppService>().ShouldBeOfType<お知らせAppService>();
            container.Resolve<Iお知らせAppService>().ShouldBeOfType<お知らせAppService>();
        }

        public interface Iお知らせAppService
        {
            
        }

        public class お知らせAppService : Iお知らせAppService
        {

        }
    }
}
