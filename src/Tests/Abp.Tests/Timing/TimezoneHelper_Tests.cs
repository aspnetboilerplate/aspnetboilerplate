using System;
using Abp.Timing.Timezone;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Timing
{
    public class TimezoneHelper_Tests
    {
        [Theory]
        [InlineData("Pacific Standard Time", "America/Los_Angeles")]
        [InlineData("Eastern Standard Time", "America/New_York")]
        [InlineData("UTC", "Etc/UTC")]
        [InlineData("Greenland Standard Time", "America/Godthab")]
        [InlineData("GMT Standard Time", "Europe/London")]
        [InlineData("W. Europe Standard Time", "Europe/Berlin")]
        [InlineData("Romance Standard Time", "Europe/Paris")]
        [InlineData("Jordan Standard Time", "Asia/Amman")]
        [InlineData("South Africa Standard Time", "Africa/Johannesburg")]
        [InlineData("Mauritius Standard Time", "Indian/Mauritius")]
        public void Windows_Timezone_Id_To_Iana_Tests(string windowsTimezoneId, string ianaTimezoneId)
        {
            TimezoneHelper.WindowsToIana(windowsTimezoneId).ShouldBe(ianaTimezoneId);
        }

        [Fact]
        public void All_Windows_Timezones_Should_Be_Convertable_To_Iana()
        {
            var allTimezones = TimeZoneInfo.GetSystemTimeZones();
            foreach (var timezone in allTimezones)
            {
                Should.NotThrow(() =>
                {
                    TimezoneHelper.WindowsToIana(timezone.Id);
                });
            }
        }

        [Fact]
        public void Should_Throw_Exception_For_Unknown_Windows_Timezone_Id()
        {
            Should.Throw<Exception>(() =>
            {
                TimezoneHelper.WindowsToIana("abc");
            });
        }
    }
}
