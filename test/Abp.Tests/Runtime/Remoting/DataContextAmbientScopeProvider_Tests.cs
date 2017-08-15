using System.Threading.Tasks;
using Abp.Runtime.Remoting;
using Shouldly;
using Xunit;

namespace Abp.Tests.Runtime.Remoting
{
    public class DataContextAmbientScopeProvider_Tests
    {
        private const string ContextKey = "Abp.Tests.TestData";

        [Fact]
        public void Test_Sync()
        {
            var scopeAccessor = new DataContextAmbientScopeProvider<TestData>(
                new AsyncLocalAmbientDataContext()
            );

            scopeAccessor.GetValue(ContextKey).ShouldBeNull();

            using (scopeAccessor.BeginScope(ContextKey, new TestData(42)))
            {
                scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);

                using (scopeAccessor.BeginScope(ContextKey, new TestData(24)))
                {
                    scopeAccessor.GetValue(ContextKey).Number.ShouldBe(24);
                }

                scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);
            }

            scopeAccessor.GetValue(ContextKey).ShouldBeNull();
        }

        [Fact]
        public async Task Test_Async()
        {
            var scopeAccessor = new DataContextAmbientScopeProvider<TestData>(
                new AsyncLocalAmbientDataContext()
            );

            scopeAccessor.GetValue(ContextKey).ShouldBeNull();

            await Task.Delay(1);

            using (scopeAccessor.BeginScope(ContextKey, new TestData(42)))
            {
                await Task.Delay(1);

                scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);

                using (scopeAccessor.BeginScope(ContextKey, new TestData(24)))
                {
                    await Task.Delay(1);

                    scopeAccessor.GetValue(ContextKey).Number.ShouldBe(24);
                }

                await Task.Delay(1);

                scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);
            }

            await Task.Delay(1);

            scopeAccessor.GetValue(ContextKey).ShouldBeNull();
        }


        public class TestData
        {
            public TestData(int number)
            {
                Number = number;
            }

            public int Number { get; set; }
        }
    }
}
