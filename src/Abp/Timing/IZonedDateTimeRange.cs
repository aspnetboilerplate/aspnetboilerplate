using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.Timing
{
    /// <summary>
    /// Defines interface for a DateTime range with timezone.
    /// </summary>
    public interface IZonedDateTimeRange : IDateTimeRange
    {
        /// <summary>
        /// The Timezone of the datetime range
        /// </summary>
        string Timezone { get; set; }

        /// <summary>
        /// The StartTime with Offset
        /// </summary>
        DateTimeOffset StartTimeOffset { get; set; }

        /// <summary>
        /// The EndTime with Offset
        /// </summary>
        DateTimeOffset EndTimeOffset { get; set; }

        /// <summary>
        /// The StartTime in UTC
        /// </summary>
        DateTime StartTimeUtc { get; set; }

        /// <summary>
        /// The EndTime in UTC
        /// </summary>
        DateTime EndTimeUtc { get; set; }
    }
}
