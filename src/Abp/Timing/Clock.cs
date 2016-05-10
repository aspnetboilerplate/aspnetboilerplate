using System;

namespace Abp.Timing
{
    /// <summary>
    ///     Used to perform some common date-time operations.
    /// </summary>
    public static class Clock
    {
        private static IClockProvider _provider;

        static Clock()
        {
            Provider = new LocalClockProvider();
        }

        /// <summary>
        ///     This object is used to perform all <see cref="Clock" /> operations.
        ///     Default value: <see cref="LocalClockProvider" />.
        /// </summary>
        public static IClockProvider Provider
        {
            get { return _provider; }
            set
            {
                if (value == null)
                {
                    throw new AbpException("Can not set Clock to null!");
                }

                _provider = value;
            }
        }

        /// <summary>
        ///     Gets Now using current <see cref="Provider" />.
        /// </summary>
        public static DateTime Now
        {
            get { return Provider.Now; }
        }

        public static DateTimeKind Kind
        {
            get { return Provider.Kind; }
        }

        /// <summary>
        ///     Normalizes given <see cref="DateTime" /> using current <see cref="Provider" />.
        /// </summary>
        /// <param name="dateTime">DateTime to be normalized.</param>
        /// <returns>Normalized DateTime</returns>
        public static DateTime Normalize(DateTime dateTime)
        {
            return Provider.Normalize(dateTime);
        }

        /// <summary>
        ///     Returns true if multiple timezone is supported, returns false if not.
        /// </summary>
        /// <returns></returns>
        public static bool SupportsMultipleTimezone()
        {
            return Provider.SupportsMultipleTimezone();
        }
    }
}