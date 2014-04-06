using System;

namespace Abp.Events.Bus.Datas
{
    /// <summary>
    /// Implements <see cref="IEventData"/> and provides a base for event data classes.
    /// </summary>
    public abstract class EventData : IEventData
    {
        public DateTime EventTime { get; set; }

        public object EventSource { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EventData()
        {
            EventTime = DateTime.Now;
        }
    }
}