using System;

namespace Abp.Timing
{
    /// <summary>
    /// Implements <see cref="IClockProvider"/> to work with UTC times.
    /// </summary>
    public class UtcClockProvider : IClockProvider
    {
        public DateTime Now => DateTime.UtcNow;

        public DateTimeKind Kind => DateTimeKind.Utc;

        public bool SupportsMultipleTimezone => true;

        public DateTime Normalize(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }

            if (dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime();
            }

            return dateTime;
        }

        internal UtcClockProvider()
        {

        }
    }
}