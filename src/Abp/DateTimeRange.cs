using System;

namespace Abp
{
    /// <summary>
    /// A basic implementation of <see cref="IDateTimeRange"/> to store a date range.
    /// </summary>
    [Serializable]
    public class DateTimeRange : IDateTimeRange
    {
        /// <summary>
        /// Start time of the datetime range.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time of the datetime range.
        /// </summary>
        public DateTime EndTime { get; set; }

        private static DateTime Now { get { return DateTime.Now; } }

        /// <summary>
        /// Creates a new <see cref="DateTimeRange"/> object.
        /// </summary>
        public DateTimeRange()
        {

        }

        /// <summary>
        /// Creates a new <see cref="DateTimeRange"/> object from given <see cref="startTime"/> and <see cref="endTime"/>.
        /// </summary>
        /// <param name="startTime">Start time of the datetime range</param>
        /// <param name="endTime">End time of the datetime range</param>
        public DateTimeRange(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// Creates a new <see cref="DateTimeRange"/> object from given <see cref="dateTimeRange"/> object.
        /// </summary>
        /// <param name="dateTimeRange">IDateTimeRange object</param>
        public DateTimeRange(IDateTimeRange dateTimeRange)
        {
            StartTime = dateTimeRange.StartTime;
            EndTime = dateTimeRange.EndTime;
        }

        /// <summary>
        /// Gets a date range represents yesterday.
        /// </summary>
        public static DateTimeRange Yesterday
        {
            get
            {
                var now = Now;
                return new DateTimeRange(now.Date.AddDays(-1), now.Date.AddMilliseconds(-1));
            }
        }

        /// <summary>
        /// Gets a date range represents today.
        /// </summary>
        public static DateTimeRange Today
        {
            get
            {
                var now = Now;
                return new DateTimeRange(now.Date, now.Date.AddDays(1).AddMilliseconds(-1));
            }
        }

        /// <summary>
        /// Gets a date range represents tomorrow.
        /// </summary>
        public static DateTimeRange Tomorrow
        {
            get
            {
                var now = Now;
                return new DateTimeRange(now.Date.AddDays(1), now.Date.AddDays(2).AddMilliseconds(-1));
            }
        }

        /// <summary>
        /// Gets a date range represents the last month.
        /// </summary>
        public static DateTimeRange LastMonth
        {
            get
            {
                var now = Now;
                var startTime = now.Date.AddDays(-now.Day + 1).AddMonths(-1);
                var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new DateTimeRange(startTime, endTime);
            }
        }

        /// <summary>
        /// Gets a date range represents this month.
        /// </summary>
        public static DateTimeRange ThisMonth
        {
            get
            {
                var now = Now;
                var startTime = now.Date.AddDays(-now.Day + 1);
                var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new DateTimeRange(startTime, endTime);
            }
        }

        /// <summary>
        /// Gets a date range represents the next month.
        /// </summary>
        public static DateTimeRange NextMonth
        {
            get
            {
                var now = Now;
                var startTime = now.Date.AddDays(-now.Day + 1).AddMonths(1);
                var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new DateTimeRange(startTime, endTime);
            }
        }
    }
}