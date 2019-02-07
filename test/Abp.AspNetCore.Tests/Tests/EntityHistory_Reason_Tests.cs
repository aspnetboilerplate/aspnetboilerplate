using System.Threading.Tasks;
using Abp.AspNetCore.EntityHistory;
using Abp.Dependency;
using Abp.EntityHistory;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class EntityHistory_Reason_Tests : AppTestBase
    {
        private readonly MyNonUseCaseMarkedClass _nonUseCaseMarkedClass;
        private readonly MyUseCaseMarkedClass _useCaseMarkedClass;

        public EntityHistory_Reason_Tests()
        {
            _nonUseCaseMarkedClass = Resolve<MyNonUseCaseMarkedClass>();
            _useCaseMarkedClass = Resolve<MyUseCaseMarkedClass>();
        }

        [Fact]
        public void HttpRequestEntityChangeSetReasonProvider_Can_Be_Constructor_Injected()
        {
            _useCaseMarkedClass.ReasonProvider.ShouldBeOfType<HttpRequestEntityChangeSetReasonProvider>();
        }

        [Fact]
        public void HttpRequestEntityChangeSetReasonProvider_Should_Be_Property_Injected()
        {
            _nonUseCaseMarkedClass.ReasonProvider.ShouldBeOfType<HttpRequestEntityChangeSetReasonProvider>();
        }

        [Fact]
        public void Should_Intercept_UseCase_Marked_Classes()
        {
            _useCaseMarkedClass.NonUseCaseMarkedMethod();
        }

        [Fact]
        public void Should_Intercept_UseCase_Marked_Methods()
        {
            _nonUseCaseMarkedClass.UseCaseMarkedMethod();
        }

        [Fact]
        public async Task Should_Intercept_UseCase_Marked_Async_Methods()
        {
            await _nonUseCaseMarkedClass.UseCaseMarkedAsyncMethod();
        }


        [Fact]
        public async Task Should_Intercept_UseCase_Marked_Async_Methods_WithResult()
        {
            await _nonUseCaseMarkedClass.UseCaseMarkedAsyncMethodWithResult();
        }

        [Fact]
        public void Should_Not_Intercept_No_UseCase_Marked_Method()
        {
            _nonUseCaseMarkedClass.AnotherMethod();
        }
    }

    public static class Consts
    {
        public const string UseCaseDescription = "UseCaseDescription";
    }

    public class MyNonUseCaseMarkedClass : ITransientDependency
    {
        public IEntityChangeSetReasonProvider ReasonProvider { get; set; }

        public MyNonUseCaseMarkedClass()
        {
            ReasonProvider = NullEntityChangeSetReasonProvider.Instance;
        }

        [UseCase(Description = Consts.UseCaseDescription)]
        public virtual void UseCaseMarkedMethod()
        {
            ReasonProvider.Reason.ShouldBe(Consts.UseCaseDescription);
        }

        [UseCase(Description = Consts.UseCaseDescription)]
        public virtual async Task UseCaseMarkedAsyncMethod()
        {
            ReasonProvider.Reason.ShouldBe(Consts.UseCaseDescription);

            await Task.CompletedTask;
        }

        [UseCase(Description = Consts.UseCaseDescription)]
        public virtual async Task<string> UseCaseMarkedAsyncMethodWithResult()
        {
            ReasonProvider.Reason.ShouldBe(Consts.UseCaseDescription);
            return await Task.FromResult("");
        }

        public virtual void AnotherMethod()
        {
            ReasonProvider.Reason.ShouldBeNull();
        }
    }

    [UseCase(Description = Consts.UseCaseDescription)]
    public class MyUseCaseMarkedClass : ITransientDependency
    {
        public readonly IEntityChangeSetReasonProvider ReasonProvider;

        public MyUseCaseMarkedClass(IEntityChangeSetReasonProvider reasonProvider)
        {
            ReasonProvider = reasonProvider;
        }

        public virtual void NonUseCaseMarkedMethod()
        {
            ReasonProvider.Reason.ShouldBe(Consts.UseCaseDescription);
        }
    }
}
