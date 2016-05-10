using System;

namespace Abp.Timing
{
    /// <summary>
    ///     Defines interface to perform some common date-time operations.
    /// </summary>
    public interface IClockProvider
    {
        /// <summary>
        ///     Gets Now.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        ///     Gets kind.
        /// </summary>
        DateTimeKind Kind { get; }

        /// <summary>
        ///     Normalizes given <see cref="DateTime" />.
        /// </summary>
        /// <param name="dateTime">DateTime to be normalized.</param>
        /// <returns>Normalized DateTime</returns>
        DateTime Normalize(DateTime dateTime);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        bool SupportsMultipleTimezone();
    }
}