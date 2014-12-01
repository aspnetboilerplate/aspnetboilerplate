using System.Reflection;
using System.Threading.Tasks;
using Abp.Reflection;
using Shouldly;
using Xunit;

namespace Abp.Tests.Reflection
{
    public class AsyncHelper_Tests
    {
        private bool _asyncMethod1Worked;
        private bool _asyncMethod2Worked;

        [Fact]
        public void IsAsync_Should_Work()
        {
            AsyncHelper.IsAsyncMethod(GetType().GetMethod("MyMethod1Sync", BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBe(false);
            AsyncHelper.IsAsyncMethod(GetType().GetMethod("MyMethod1Async", BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBe(true);
            AsyncHelper.IsAsyncMethod(GetType().GetMethod("MyMethod2Sync", BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBe(false);
            AsyncHelper.IsAsyncMethod(GetType().GetMethod("MyMethod2Async", BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBe(true);
        }

        [Fact]
        public void Should_Call_AfterAction()
        {
            _asyncMethod1Worked.ShouldBe(false);
            AsyncHelper.ReturnTaskAfterAction(
                MyMethod1Async(),
                async () =>
                {
                    _asyncMethod1Worked.ShouldBe(true);
                    await Task.Delay(10);
                }).Wait();
            
            _asyncMethod2Worked.ShouldBe(false);
            var returnValue = AsyncHelper.ReturnGenericTaskAfterAction(
                MyMethod2Async(),
                async () =>
                {
                    _asyncMethod2Worked.ShouldBe(true);
                    await Task.Delay(10);
                }).Result;

            returnValue.ShouldBe(42);
        }

        private async Task MyMethod1Async()
        {
            await Task.Delay(10);
            _asyncMethod1Worked = true;
        }

        private async Task<int> MyMethod2Async()
        {
            await Task.Delay(10);
            _asyncMethod2Worked = true;
            return 42;
        }

        private void MyMethod1Sync()
        {
            _asyncMethod1Worked = true;
        }

        private void MyMethod2Sync()
        {

        }
    }
}
