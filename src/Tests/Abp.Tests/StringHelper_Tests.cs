using Abp.Collections.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests
{
    public class StringHelper_Tests
    {
        [Fact]
        public void Should_ParseExact()
        {
            Should_ParseExact("My name is Neo.", "My name is Neo.");
            Should_ParseExact("My name is Neo.", "My name is {0}.", "Neo");
            Should_ParseExact("2/17/2009 10:57:42 AM...Executing file 26 of 81 files", "{0}...Executing file {1} of {2} files", "2/17/2009 10:57:42 AM", "26", "81");
        }

        private static void Should_ParseExact(string str, string format, params string[] expectedValues)
        {
            string[] actualValues;
            StringHelper.TryParseExact(str, format, out actualValues).ShouldBe(true);

            if (expectedValues.IsNullOrEmpty())
            {
                actualValues.Length.ShouldBe(0);
                return;
            }

            actualValues.Length.ShouldBe(expectedValues.Length);
            for (var i = 0; i < actualValues.Length; i++)
            {
                var actualValue = actualValues[i];
                var expectedValue = expectedValues[i];
                actualValue.ShouldBe(expectedValue);
            }
        }
    }
}
