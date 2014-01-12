using System;

namespace Abp.Events.Bus.Datas
{
    /// <summary>
    /// All event data objects must be inherited by this class.
    /// </summary>
    public abstract class EventData : IEventData
    {
        public DateTime EventTime { get; set; }

        public object EventSource { get; set; }

        protected EventData()
        {
            EventTime = DateTime.Now;
        }
    }
}