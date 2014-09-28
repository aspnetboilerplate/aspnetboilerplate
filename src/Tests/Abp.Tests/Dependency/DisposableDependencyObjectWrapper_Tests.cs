using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class DisposableDependencyObjectWrapper_Tests : TestBaseWithSelfIocManager
    {
        [Fact]
        public void ResolveAsDisposable_Should_Work()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<SimpleDisposableObject>().LifestyleTransient()
                );

            SimpleDisposableObject simpleObj;

            using (var wrapper = LocalIocManager.ResolveAsDisposable<SimpleDisposableObject>())
            {
                wrapper.Object.ShouldNotBe(null);
                simpleObj = wrapper.Object;
            }

            simpleObj.DisposeCount.ShouldBe(1);
        }
    }
}
