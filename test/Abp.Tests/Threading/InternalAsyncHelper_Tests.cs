using System;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Threading;
using Shouldly;
using Xunit;

namespace Abp.Tests.Threading
{
    public class InternalAsyncHelper_Tests
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
        public async Task Should_Call_AfterAction()
        {
            _asyncMethod1Worked.ShouldBe(false);
            await InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                MyMethod1Async(),
                async () =>
                {
                    _asyncMethod1Worked.ShouldBe(true);
                    await Task.Delay(10);
                },
                (exception) => { }
                );

            _asyncMethod2Worked.ShouldBe(false);
            var returnValue = await InternalAsyncHelper.AwaitTaskWithPostActionAndFinallyAndGetResult(
                MyMethod2Async(),
                async () =>
                {
                    _asyncMethod2Worked.ShouldBe(true);
                    await Task.Delay(10);
                },
                (exception) => { }
                );

            returnValue.ShouldBe(42);
        }

        [Fact]
        public async Task Should_Call_Finally_On_Success()
        {
            var calledFinally = false;

            await InternalAsyncHelper.AwaitTaskWithFinally(
                MyMethod1Async(),
                exception =>
                {
                    calledFinally = true;
                    exception.ShouldBe(null);
                });

            calledFinally.ShouldBe(true);
        }
        
        [Fact]
        public async Task Should_Call_Finally_On_Exception()
        {
            var calledFinally = false;

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await InternalAsyncHelper.AwaitTaskWithFinally(
                    MyMethod1Async(true),
                    exception =>
                    {
                        calledFinally = true;
                        exception.ShouldNotBe(null);
                        exception.Message.ShouldBe("test exception");
                    });
            });

            calledFinally.ShouldBe(true);
        }

        private async Task MyMethod1Async(bool throwEx = false)
        {
            await Task.Delay(10);
            _asyncMethod1Worked = true;
            if (throwEx)
            {
                throw new Exception("test exception");
            }
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
