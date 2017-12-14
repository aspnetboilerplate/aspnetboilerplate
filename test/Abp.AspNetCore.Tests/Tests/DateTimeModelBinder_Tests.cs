using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.Timing;
using Abp.Web.Models;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class DateTimeModelBinder_Tests : AppTestBase
    {
        [Theory]
        [InlineData("2016-04-13T08:58:10.526Z", "utc")]
        [InlineData("2016-04-13T08:58:10.526", "local")]
        [InlineData("2016-04-13 08:58:10.526Z", "utc")]
        [InlineData("2016-04-13 08:58:10.526", "local")]
        public async Task Controller_Should_Receive_Correct_DateTimeKind_For_Current_ClockProvider(string date, string expectedKind)
        {
            Clock.Provider = StringToClockProvider(expectedKind);

            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetDateTimeKind),
                    new
                    {
                        date = date
                    }
                )
            );

            response.Result.ShouldBe(expectedKind.ToLower());
        }

        private IClockProvider StringToClockProvider(string dateTimeKind)
        {
            if (dateTimeKind == "local")
            {
                return ClockProviders.Local;
            }

            if (dateTimeKind == "utc")
            {
                return ClockProviders.Utc;
            }

            return ClockProviders.Unspecified;
        }
    }
}
