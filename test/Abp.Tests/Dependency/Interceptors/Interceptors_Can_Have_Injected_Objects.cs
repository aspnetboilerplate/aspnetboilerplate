using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency.Interceptors
{
    public class Interceptors_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Interceptors_Should_Work()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<BracketInterceptor>().LifestyleTransient(),
                Component.For<MyGreetingClass>().Interceptors<BracketInterceptor>().LifestyleTransient()
                );

            var greetingObj = LocalIocManager.Resolve<MyGreetingClass>();

            greetingObj.SayHello("Halil").ShouldBe("(Hello Halil)");
        }

        public class MyGreetingClass
        {
            public virtual string SayHello(string name)
            {
                return "Hello " + name;
            }
        }

        public class BracketInterceptor : CastleAbpInterceptorAdapter<BracketInterceptor>
        {
            protected override void InterceptSync(IAbpMethodInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = "(" + invocation.ReturnValue + ")";
            }
        }
    }
}
