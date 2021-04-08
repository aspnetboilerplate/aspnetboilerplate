using Abp.Threading;
using Shouldly;
using Xunit;

namespace Abp.Tests.Threading
{
    public class OneTimeRunner_Tests
    {
        [Fact]
        public void OneTimeRunner_Should_Run_Once()
        {
            var oneTimeRunner = new OneTimeRunner();
            var count = 0;
            for (int i = 0; i < 10; i++)
            {
                oneTimeRunner.Run(() =>
                {
                    count++;
                });
            }
            
            count.ShouldBe(1);
        }
    }
}
