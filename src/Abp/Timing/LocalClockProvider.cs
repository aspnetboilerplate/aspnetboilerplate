using System;

namespace Abp.Timing
{
    /// <summary>
    ///     Implements <see cref="IClockProvider" /> to work with local times.
    /// </summary>
    public class LocalClockProvider : IClockProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTimeKind Kind
        {
            get { return DateTimeKind.Local; }
        }

        public DateTime Normalize(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            }

            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToLocalTime();
            }

            return dateTime;
        }

        public bool SupportsMultipleTimezone()
        {
            return false;
        }
    }
}