using Shouldly;
using Xunit;

namespace Abp.Tests
{
    public class DisposeAction_Test
    {
        [Fact]
        public void Should_Call_Action_When_Disposed()
        {
            var actionIsCalled = false;
            
            using (new DisposeAction(() => actionIsCalled = true))
            {
                
            }

            actionIsCalled.ShouldBe(true);
        }
    }
}
