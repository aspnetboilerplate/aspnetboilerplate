using System;
using System.Threading.Tasks;
using Abp.Events.Bus.Datas;
using Abp.Events.Bus.Factories;
using Abp.Events.Bus.Handlers;
using Abp.Utils.Etc;

namespace Abp.Events.Bus
{
    /// <summary>
    /// An event bus that implements Null object pattern.
    /// </summary>
    public class NullEventBus : IEventBus
    {
        /// <summary>
        /// Gets single instance of <see cref="NullEventBus"/> class.
        /// </summary>
        public static NullEventBus Instance { get { return SingletonInstance; } }
        private static readonly NullEventBus SingletonInstance = new NullEventBus();
        
        private NullEventBus()
        {
            
        }

        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return NullDisposable.Instance;            
        }

        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler<TEventData>, new()
        {
            return NullDisposable.Instance;
        }

        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return NullDisposable.Instance;
        }

        public IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        public IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory)
        {
            return NullDisposable.Instance;
        }

        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            
        }

        public void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            
        }

        public void Unregister(Type eventType, IEventHandler handler)
        {
            
        }

        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            
        }

        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
            
        }

        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
            
        }

        public void UnregisterAll(Type eventType)
        {
            
        }

        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            
        }

        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            
        }

        public void Trigger(Type eventType, EventData eventData)
        {
            
        }

        public void Trigger(Type eventType, object eventSource, EventData eventData)
        {
            
        }

        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return new Task(() => {});
        }

        public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            return new Task(() => { });            
        }

        public Task TriggerAsync(Type eventType, EventData eventData)
        {
            return new Task(() => { });
        }

        public Task TriggerAsync(Type eventType, object eventSource, EventData eventData)
        {
            return new Task(() => { });
        }
    }
}
