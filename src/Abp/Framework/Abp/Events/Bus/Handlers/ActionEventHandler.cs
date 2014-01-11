using System;

namespace Abp.Events.Bus.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    public class ActionEventHandler<TEventData> : IEventHandler<TEventData>
    {
        private readonly Action<TEventData> _handler;

        public ActionEventHandler(Action<TEventData> handler)
        {
            _handler = handler;
        }

        public void HandleEvent(TEventData eventData)
        {
            _handler(eventData);
        }
    }
}