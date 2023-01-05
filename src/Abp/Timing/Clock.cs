using System;
using Abp.Dependency;

namespace Abp.Timing
{
    /// <summary>
    /// Used to perform some common date-time operations.
    /// </summary>
    public class Clock : IClock, ITransientDependency
    {
        /// <summary>
        /// This object is used to perform all <see cref="Clock"/> operations.
        /// Default value: <see cref="UnspecifiedClockProvider"/>.
        /// </summary>
        public IClockProvider Provider
        {
            get { return _provider; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Can not set Clock.Provider to null!");
                }

                _provider = value;
            }
        }

        private IClockProvider _provider;

        public Clock()
        {
            Provider = ClockProviders.Unspecified;
        }

        /// <summary>
        /// Gets Now using current <see cref="Provider"/>.
        /// </summary>
        public DateTime Now => Provider.Now;

        public DateTimeKind Kind => Provider.Kind;

        /// <summary>
        /// Returns true if multiple timezone is supported, returns false if not.
        /// </summary>
        public bool SupportsMultipleTimezone => Provider.SupportsMultipleTimezone;

        /// <summary>
        /// Normalizes given <see cref="DateTime"/> using current <see cref="Provider"/>.
        /// </summary>
        /// <param name="dateTime">DateTime to be normalized.</param>
        /// <returns>Normalized DateTime</returns>
        public DateTime Normalize(DateTime dateTime)
        {
            return Provider.Normalize(dateTime);
        }
    }
}