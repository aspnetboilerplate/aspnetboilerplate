using System;
using Abp.Timing.Timezone;

namespace Abp.Timing
{
    /// <summary>
    /// A basic implementation of <see cref="IZonedDateTimeRange"/> to store a date range with a timezone.
    /// Default timezone is UTC
    /// </summary>
    public class ZonedDateTimeRange : DateTimeRange, IZonedDateTimeRange
    {
        public ZonedDateTimeRange()
        {
            
        }

        public ZonedDateTimeRange(string timezone)
        {
            Timezone = timezone;
        }

        public ZonedDateTimeRange(IDateTimeRange dateTimeRange, string timeZoneId) : base(dateTimeRange)
        {
            Timezone = timeZoneId;
        }

        public ZonedDateTimeRange(IZonedDateTimeRange zonedDateTimeRange) : base(zonedDateTimeRange)
        {
            Timezone = zonedDateTimeRange.Timezone;
        }

        public ZonedDateTimeRange(DateTime startTime, DateTime endTime, string timeZoneId) : base(startTime, endTime)
        {
            Timezone = timeZoneId;
        }

        public ZonedDateTimeRange(DateTime startTime, TimeSpan timeSpan, string timeZoneId) : base(startTime, timeSpan)
        {
            Timezone = timeZoneId;
        }

        /// <summary>
        /// The Timezone of the datetime range
        /// </summary>
        public string Timezone { get; set; } = "UTC";

        /// <summary>
        /// The StartTime with Offset
        /// </summary>
        public DateTimeOffset StartTimeOffset
        {
            get => TimezoneHelper.ConvertToDateTimeOffset(StartTime, Timezone);
            set => StartTimeUtc = value.UtcDateTime;
        }

        /// <summary>
        /// The EndTime with Offset
        /// </summary>
        public DateTimeOffset EndTimeOffset
        {
            get => TimezoneHelper.ConvertToDateTimeOffset(EndTime, Timezone);
            set => EndTimeUtc = value.UtcDateTime;
        }

        /// <summary>
        /// The StartTime in UTC
        /// </summary>
        public DateTime StartTimeUtc
        {
            get => StartTimeOffset.UtcDateTime;
            set
            {
                var localized = TimezoneHelper.ConvertFromUtc(value, Timezone);
                if (localized.HasValue)
                {
                    StartTime = localized.Value;
                }
            }
        }

        /// <summary>
        /// The EndTime in UTC
        /// </summary>
        public DateTime EndTimeUtc
        {
            get => EndTimeOffset.UtcDateTime;
            set
            {
                var localized = TimezoneHelper.ConvertFromUtc(value, Timezone);
                if (localized.HasValue)
                {
                    EndTime = localized.Value;
                }
            }
        }

        /// <summary>
        /// The current time based on the timezone
        /// </summary>
        public DateTime Now
        {
            get
            {
                DateTime? localTime;
                switch (Clock.Kind)
                {
                    case DateTimeKind.Local:
                        localTime = TimezoneHelper.ConvertFromUtc(Clock.Now.ToUniversalTime(), Timezone);
                        break;
                    case DateTimeKind.Unspecified:
                        localTime = Clock.Now;
                        break;
                    case DateTimeKind.Utc:
                        localTime = TimezoneHelper.ConvertFromUtc(Clock.Now, Timezone);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return localTime ?? Clock.Now;
            }
        }

        /// <summary>
        /// Gets a zoned date range representing yesterday.
        /// </summary>
        public new ZonedDateTimeRange Yesterday
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(now.Date.AddDays(-1), now.Date.AddMilliseconds(-1), Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing today.
        /// </summary>
        public new ZonedDateTimeRange Today
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(now.Date, now.Date.AddDays(1).AddMilliseconds(-1), Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing tomorrow.
        /// </summary>
        public new ZonedDateTimeRange Tomorrow
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(now.Date.AddDays(1), now.Date.AddDays(2).AddMilliseconds(-1), Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing the last month.
        /// </summary>
        public new ZonedDateTimeRange LastMonth
        {
            get
            {
                var now = Now;
                var startTime = now.Date.AddDays(-now.Day + 1).AddMonths(-1);
                var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new ZonedDateTimeRange(startTime, endTime, Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing this month.
        /// </summary>
        public new ZonedDateTimeRange ThisMonth
        {
            get
            {
                var now = Now;
                var startTime = now.Date.AddDays(-now.Day + 1);
                var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new ZonedDateTimeRange(startTime, endTime, Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing the next month.
        /// </summary>
        public new ZonedDateTimeRange NextMonth
        {
            get
            {
                var now = Now;
                var startTime = now.Date.AddDays(-now.Day + 1).AddMonths(1);
                var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new ZonedDateTimeRange(startTime, endTime, Timezone);
            }
        }


        /// <summary>
        /// Gets a zoned date range representing the last year.
        /// </summary>
        public new ZonedDateTimeRange LastYear
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(new DateTime(now.Year - 1, 1, 1), new DateTime(now.Year, 1, 1).AddMilliseconds(-1), Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing this year.
        /// </summary>
        public new ZonedDateTimeRange ThisYear
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(new DateTime(now.Year, 1, 1), new DateTime(now.Year + 1, 1, 1).AddMilliseconds(-1), Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing the next year.
        /// </summary>
        public new ZonedDateTimeRange NextYear
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(new DateTime(now.Year + 1, 1, 1), new DateTime(now.Year + 2, 1, 1).AddMilliseconds(-1), Timezone);
            }
        }


        /// <summary>
        /// Gets a zoned date range representing the last 30 days (30x24 hours) including today.
        /// </summary>
        public ZonedDateTimeRange Last30Days
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(now.AddDays(-30), now, Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing the last 30 days excluding today.
        /// </summary>
        public ZonedDateTimeRange Last30DaysExceptToday
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(now.Date.AddDays(-30), now.Date.AddMilliseconds(-1), Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing the last 7 days (7x24 hours) including today.
        /// </summary>
        public ZonedDateTimeRange Last7Days
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(now.AddDays(-7), now, Timezone);
            }
        }

        /// <summary>
        /// Gets a zoned date range representing the last 7 days excluding today.
        /// </summary>
        public ZonedDateTimeRange Last7DaysExceptToday
        {
            get
            {
                var now = Now;
                return new ZonedDateTimeRange(now.Date.AddDays(-7), now.Date.AddMilliseconds(-1), Timezone);
            }
        }
    }
}
