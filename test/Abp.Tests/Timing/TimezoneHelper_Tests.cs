using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
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
        [InlineData("Malay Peninsula Standard Time", "Asia/Kuala_Lumpur")]
        [InlineData("Qyzylorda Standard Time", "Asia/Qyzylorda")]
        [InlineData("UTC-11", "Etc/GMT+11")]
        [InlineData("UTC-09", "Etc/GMT+9")]
        [InlineData("UTC-08", "Etc/GMT+8")]
        [InlineData("UTC-02", "Etc/GMT+2")]
        [InlineData("UTC+12", "Etc/GMT-12")]
        [InlineData("UTC+13", "Etc/GMT-13")]
        public void Windows_Timezone_Id_To_Iana_Tests(string windowsTimezoneId, string ianaTimezoneId)
        {
            TimezoneHelper.WindowsToIana(windowsTimezoneId).ShouldBe(ianaTimezoneId);
        }

        [Theory]
        [InlineData("America/Los_Angeles", "Pacific Standard Time")]
        [InlineData("America/Argentina/San_Luis", "Argentina Standard Time")]
        [InlineData("Etc/UTC", "UTC")]
        [InlineData("America/Godthab", "Greenland Standard Time")]
        [InlineData("Europe/London", "GMT Standard Time")]
        [InlineData("Europe/Berlin", "W. Europe Standard Time")]
        [InlineData("Europe/Paris", "Romance Standard Time")]
        [InlineData("Asia/Amman", "Jordan Standard Time")]
        [InlineData("Europe/Zaporozhye", "FLE Standard Time")]
        [InlineData("Asia/Choibalsan", "Ulaanbaatar Standard Time")]
        [InlineData("Etc/GMT+9", "UTC-09")]
        [InlineData("Etc/GMT+8", "UTC-08")]
        public void Iana_Timezone_Id_To_Windows_Tests(string ianaTimezoneId, string windowsTimezoneId)
        {
            TimezoneHelper.IanaToWindows(ianaTimezoneId).ShouldBe(windowsTimezoneId);
        }

        /// <summary>
        /// The sign of an <c>Etc/GMT±X</c> IANA zone is inverted compared to the UTC offset it represents,
        /// so <c>UTC-09</c> must map to <c>Etc/GMT+9</c> and not to <c>Etc/GMT-9</c>.
        /// </summary>
        [Theory]
        [InlineData("UTC-11", -11)]
        [InlineData("UTC-09", -9)]
        [InlineData("UTC-08", -8)]
        [InlineData("UTC-02", -2)]
        [InlineData("UTC+12", 12)]
        [InlineData("UTC+13", 13)]
        public void Windows_Utc_Offset_Timezones_Should_Map_To_Iana_Zones_With_The_Same_Offset(
            string windowsTimezoneId,
            int expectedOffsetInHours)
        {
            var ianaTimezoneId = TimezoneHelper.WindowsToIana(windowsTimezoneId);

            TimezoneHelper.FindTimeZoneInfo(ianaTimezoneId)
                .BaseUtcOffset
                .ShouldBe(TimeSpan.FromHours(expectedOffsetInHours));
        }

        [Fact]
        public void All_Windows_Timezones_Should_Be_Convertable_To_Iana()
        {
            var allTimezones = TimezoneHelper.GetWindowsTimeZoneIds();

            Should.NotThrow(() =>
            {
                var exceptions = new List<string>();

                foreach (var timezone in allTimezones)
                {
                    try
                    {
                        TimezoneHelper.WindowsToIana(timezone);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex.Message);
                    }
                }

                if (exceptions.Any())
                {
                    throw new Exception(exceptions.JoinAsString(Environment.NewLine));
                }
            });
        }

        [Fact]
        public void Should_Throw_Exception_For_Unknown_Windows_Timezone_Id()
        {
            Should.Throw<Exception>(() =>
            {
                TimezoneHelper.WindowsToIana("abc");
            });
        }

        [Fact]
        public void Should_Throw_Exception_For_Unknown_Iana_Timezone_Id()
        {
            Should.Throw<Exception>(() =>
            {
                TimezoneHelper.IanaToWindows("cba");
            });
        }

        [Fact]
        public void Convert_By_Iana_Timezone_Should_Be_Convert_By_Windows_Timezone()
        {
            var now = DateTime.UtcNow;
            TimezoneHelper.ConvertTimeFromUtcByIanaTimeZoneId(now, "Asia/Shanghai")
                .ShouldBe(TimezoneHelper.ConvertFromUtc(now, "China Standard Time"));
        }

        [Fact]
        public void ConvertToDateTimeOffset_Date_With_America_NewYork_TimeZone_Should_Return_Correct_DateTimeOffset()
        {
            var testDate = new DateTime(1980,11,20);
            var timeSpan = new TimeSpan(-5,0,0);

            DateTimeOffset? dateTimeOffset = TimezoneHelper.ConvertToDateTimeOffset(testDate, "America/New_York");

            dateTimeOffset.ShouldNotBe(null);
            dateTimeOffset.Value.Offset.ShouldBe(timeSpan);
            dateTimeOffset.Value.DateTime.ShouldBe(testDate);
        }

        [Fact]
        public void ConvertToDateTimeOffset_Date_With_America_NewYork_TimeZone_Should_Return_Correct_DateTimeOffset_With_DaylightSavings()
        {
            var testDate = new DateTime(1980, 5, 20);
            var timeSpan = new TimeSpan(-4, 0, 0);

            DateTimeOffset? dateTimeOffset = TimezoneHelper.ConvertToDateTimeOffset(testDate, "America/New_York");

            dateTimeOffset.ShouldNotBe(null);
            dateTimeOffset.Value.Offset.ShouldBe(timeSpan);
            dateTimeOffset.Value.DateTime.ShouldBe(testDate);
        }

        [Fact]
        public void ConvertToDateTimeOffset_Dates_With_America_Phoenix_TimeZone_Should_Return_Correct_DateTimeOffsests_With_No_DaylightSavings()
        {
            var testDate = new DateTime(1980, 5, 20);
            var timeSpan = new TimeSpan(-7, 0, 0);

            DateTimeOffset? dateTimeOffset = TimezoneHelper.ConvertToDateTimeOffset(testDate, "America/Phoenix");

            dateTimeOffset.ShouldNotBe(null);
            dateTimeOffset.Value.Offset.ShouldBe(timeSpan);
            dateTimeOffset.Value.DateTime.ShouldBe(testDate);

            var testDate2 = new DateTime(1980, 11, 20);

            DateTimeOffset? dateTimeOffset2 = TimezoneHelper.ConvertToDateTimeOffset(testDate2, "America/Phoenix");

            dateTimeOffset2.ShouldNotBe(null);
            dateTimeOffset2.Value.Offset.ShouldBe(timeSpan); // should be the same timespan as previous date
            dateTimeOffset2.Value.DateTime.ShouldBe(testDate2);
        }
    }
}
