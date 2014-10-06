using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class DisposableDependencyObjectWrapper_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ResolveAsDisposable_Should_Work()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);

            SimpleDisposableObject simpleObj;

            using (var wrapper = LocalIocManager.ResolveAsDisposable<SimpleDisposableObject>())
            {
                wrapper.Object.ShouldNotBe(null);
                simpleObj = wrapper.Object;
            }

            simpleObj.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public void ResolveAsDisposable_With_Constructor_Args_Should_Work()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);

            using (var wrapper = LocalIocManager.ResolveAsDisposable<SimpleDisposableObject>(new { myData = 42 }))
            {
                wrapper.Object.MyData.ShouldBe(42);
            }
        }
    }
}
