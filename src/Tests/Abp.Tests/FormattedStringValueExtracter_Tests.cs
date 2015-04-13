using Abp.Text;
using Shouldly;
using Xunit;

namespace Abp.Tests
{
    public class FormattedStringValueExtracter_Tests
    {
        [Fact]
        public void Test1()
        {
            var extracter = new FormattedStringValueExtracter();
            var result = extracter.Extract("My name is Neo.", "My name is {0}.");
            result.Matched.ShouldBe(true);
            result.Values[0].ShouldBe("Neo");
        }
    }
}