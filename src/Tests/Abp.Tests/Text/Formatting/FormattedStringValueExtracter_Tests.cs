using Abp.Text;
using Shouldly;
using Xunit;

namespace Abp.Tests.Text.Formatting
{
    public class FormattedStringValueExtracter_Tests
    {
        [Fact]
        public void Test_Matched()
        {
            Test_Matched(
                "My name is Neo.",
                "My name is {0}.",
                new NameValue("0", "Neo")
                );
        }

        [Fact]
        public void Test_Not_Matched()
        {
            Test_Not_Matched(
                "My name is Neo.",
                "My name is Marry."
                );
        }

        private void Test_Matched(string str, string format, params NameValue[] expectedPairs)
        {
            var result = new FormattedStringValueExtracter().Extract(str, format);
            result.IsMatched.ShouldBe(true);

            if (expectedPairs == null)
            {
                result.Matches.Count.ShouldBe(0);
                return;
            }

            result.Matches.Count.ShouldBe(expectedPairs.Length);

            for (int i = 0; i < expectedPairs.Length; i++)
            {
                var actualMatch = result.Matches[i];
                var expectedPair = expectedPairs[i];

                actualMatch.Name.ShouldBe(expectedPair.Name);
                actualMatch.Value.ShouldBe(expectedPair.Value);
            }
        }

        private void Test_Not_Matched(string str, string format)
        {
            var result = new FormattedStringValueExtracter().Extract(str, format);
            result.IsMatched.ShouldBe(false);
        }
    }
}