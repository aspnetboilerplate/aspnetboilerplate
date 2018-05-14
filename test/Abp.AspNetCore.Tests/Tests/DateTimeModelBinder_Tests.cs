using System;
using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.AspNetCore.App.Models;
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

        [Theory]
        [InlineData("2016-04-13T08:58:10.526Z", "local")]
        [InlineData("2016-04-13T08:58:10.526", "unspecified")]
        [InlineData("2016-04-13 08:58:10.526Z", "local")]
        [InlineData("2016-04-13 08:58:10.526", "unspecified")]
        [InlineData("2018-01-18T10:41:52.3257108+03:00", "local")]
        public async Task Controller_Should_Receive_Correct_DateTimeKind_For_Current_ClockProvider_When_Not_Normalized_Property(string date, string expectedKind)
        {
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetNotNormalizedDateTimeKindProperty),
                    new
                    {
                        date = WebUtility.UrlEncode(date)
                    }
                )
            );

            response.Result.ToLower().ShouldBe(expectedKind.ToLower());
        }

        [Theory]
        [InlineData("2016-04-13T08:58:10.526Z", "local")]
        [InlineData("2016-04-13T08:58:10.526", "unspecified")]
        [InlineData("2016-04-13 08:58:10.526Z", "local")]
        [InlineData("2016-04-13 08:58:10.526", "unspecified")]
        [InlineData("2018-01-18T10:41:52.3257108+03:00", "local")]
        public async Task Controller_Should_Return_Correct_DateTimeKind_For_Not_Normalized_DateTime_Property(string date, string expectedKind)
        {
            Clock.Provider = ClockProviders.Utc;

            var response2 = await GetResponseAsObjectAsync<AjaxResponse<SimpleDateModel2>>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetNotNormalizedDateTimeKindProperty2),
                    new
                    {
                        date = WebUtility.UrlEncode(date)
                    }
                )
            );

            response2.Result.Date.Kind.ShouldBe(StringToClockProvider(expectedKind.ToLower()).Kind);

            var response3 = await GetResponseAsObjectAsync<AjaxResponse<SimpleDateModel3>>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetNotNormalizedDateTimeKindProperty3),
                    new
                    {
                        date = WebUtility.UrlEncode(date)
                    }
                )
            );

            response3.Result.Date.Kind.ShouldBe(StringToClockProvider(expectedKind.ToLower()).Kind);
        }

        [Theory]
        [InlineData("2016-04-13T08:58:10.526Z")]
        public async Task Controller_Should_Not_Throw_Exception_When_Correct_DateTime_Is_Provided(string date)
        {
            Clock.Provider = ClockProviders.Utc;

            await GetResponseAsObjectAsync<AjaxResponse<SimpleDateModel4>>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetNotNormalizedDateTimeKindProperty4),
                    new
                    {
                        date = WebUtility.UrlEncode(date)
                    }
                )
            );
        }

        [Theory]
        [InlineData("2016-04-13T08:58:10.526Z", "local")]
        [InlineData("2016-04-13T08:58:10.526", "unspecified")]
        [InlineData("2016-04-13 08:58:10.526Z", "local")]
        [InlineData("2016-04-13 08:58:10.526", "unspecified")]
        [InlineData("2018-01-18T10:41:52.3257108+03:00", "local")]
        public async Task Controller_Should_Receive_Correct_DateTimeKind_For_Current_ClockProvider_When_Not_Normalized_Class(string date, string expectedKind)
        {
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetNotNormalizedDateTimeKindClass),
                    new
                    {
                        date = WebUtility.UrlEncode(date)
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
