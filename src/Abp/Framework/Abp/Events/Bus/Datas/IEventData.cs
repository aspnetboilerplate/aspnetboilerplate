using System;

namespace Abp.Events.Bus.Datas
{
    /// <summary>
    /// Defines interface for all Event data classes.
    /// </summary>
    public interface IEventData
    {
        /// <summary>
        /// The time when the event occured.
        /// </summary>
        DateTime EventTime { get; set; }

        /// <summary>
        /// The object which triggers the event.
        /// </summary>
        object EventSource { get; set; }
    }
}
