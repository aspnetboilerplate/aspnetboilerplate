using System;
using Abp.Events.Bus.Handlers;

namespace Abp.Events.Bus.Factories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    public class ActionHandlerFactory<TEventData> : IEventHandlerFactory
    {
        private readonly ActionEventHandler<TEventData> _handler;

        public ActionHandlerFactory(Action<TEventData> handlerAction)
            : this(new ActionEventHandler<TEventData>(handlerAction))
        {
        }

        public ActionHandlerFactory(ActionEventHandler<TEventData> handler)
        {
            _handler = handler;
        }

        public IEventHandler GetHandler()
        {
            return _handler;
        }

        public void ReleaseHandler(IEventHandler handler)
        {
            
        }
    }
}