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

        /// <summary>
        /// Creates a new <see cref="DateTimeRange"/> object.
        /// </summary>
        public DateTimeRange()
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="DateTimeRange"/> object.
        /// </summary>
        /// <param name="startTime">Start time of the datetime range</param>
        /// <param name="endTime">End time of the datetime range</param>
        public DateTimeRange(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}