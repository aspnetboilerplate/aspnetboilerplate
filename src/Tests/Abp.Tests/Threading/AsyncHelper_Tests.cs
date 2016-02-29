using System.Threading.Tasks;
using Adorable.Threading;
using Shouldly;
using Xunit;

namespace Adorable.Tests.Threading
{
    public class AsyncHelper_Tests
    {
        [Fact]
        public void Test1()
        {
            AsyncHelper.RunSync(AsyncMethod1);
            AsyncHelper.RunSync(() => AsyncMethod2(21)).ShouldBe(42);
        }

        private async Task AsyncMethod1()
        {
            await Task.Delay(10);
        }

        private async Task<int> AsyncMethod2(int p)
        {
            await Task.Delay(10);
            return p * 2;
        }
    }
}
