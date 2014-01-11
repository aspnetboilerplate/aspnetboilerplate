using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Events.Bus.Datas;
using Abp.Events.Bus.Factories;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;

namespace Abp.Events.Bus
{
    /// <summary>
    /// Implements EventBus as Singleton pattern.
    /// </summary>
    public class EventBus : IEventBus
    {
        #region Private fields

        private static readonly Dictionary<Type, List<IEventHandlerFactory>> HandlerFactories;

        /// <summary>
        /// Reference to the Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        #endregion

        #region Constructor

        static EventBus()
        {
            HandlerFactories = new Dictionary<Type, List<IEventHandlerFactory>>();
        }

        #endregion

        #region Public methods

        #region Register

        public void Register<TEventData>(IEventHandler<TEventData> handler)
        {
            Register(typeof(TEventData), new SingleInstanceHandlerFactory(handler));
        }

        public void Register(Type eventType, IEventHandler handler)
        {
            Register(eventType, new SingleInstanceHandlerFactory(handler));
        }

        public void Register<TEventData>(IEventHandlerFactory handlerFactory)
        {
            Register(typeof(TEventData), handlerFactory);
        }

        public void Register(Type eventType, IEventHandlerFactory handlerFactory)
        {
            lock (HandlerFactories)
            {
                GetOrCreateHandlerFactories(eventType).Add(handlerFactory);
            }
        }

        public void Register<TEventData>(Action<TEventData> handler)
        {
            Register(typeof(TEventData), new ActionHandlerFactory<TEventData>(handler));
        }

        #endregion

        #region Unregister

        public void Unregister<TEventData>(IEventHandler<TEventData> handler)
        {
            Unregister(typeof(TEventData), handler);
        }

        public void Unregister(Type eventType, IEventHandler handler)
        {
            lock (HandlerFactories)
            {
                var removingFactories =
                    GetOrCreateHandlerFactories(eventType)
                        .OfType<SingleInstanceHandlerFactory>()
                        .Where(instanceFactory => instanceFactory.HandlerInstance == handler)
                        .ToList();

                foreach (var instanceFactory in removingFactories)
                {
                    Unregister(eventType, instanceFactory);
                }
            }
        }

        public void Unregister<TEventData>(IEventHandlerFactory factory)
        {
            Unregister(typeof(TEventData), factory);
        }

        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
            lock (HandlerFactories)
            {
                GetOrCreateHandlerFactories(eventType).Remove(factory);
            }
        }

        public void UnregisterAll<TEventData>()
        {
            UnregisterAll(typeof(TEventData));
        }

        public void UnregisterAll(Type eventType)
        {
            lock (HandlerFactories)
            {
                GetOrCreateHandlerFactories(eventType).Clear();
            }
        }

        #endregion

        #region Trigger

        public void Trigger<TEventData>(TEventData eventData)
        {
            IEventHandlerFactory[] factoriesToTrigger;
            lock (HandlerFactories)
            {
                List<IEventHandlerFactory> handlerFactoryList;
                if (!HandlerFactories.TryGetValue(typeof(TEventData), out handlerFactoryList))
                {
                    return;
                }

                factoriesToTrigger = handlerFactoryList.ToArray();
            }

            foreach (var factoryToTrigger in factoriesToTrigger)
            {
                var eventHandler = factoryToTrigger.GetHandler() as IEventHandler<TEventData>;
                if (eventHandler == null)
                {
                    throw new Exception("Registered event handler for event type " + typeof(TEventData).Name + " does not implement IEventHandler<" + typeof(TEventData).Name + "> interface!");
                }

                try
                {
                    eventHandler.HandleEvent(eventData);
                }
                finally
                {
                    factoryToTrigger.ReleaseHandler(eventHandler);
                }
            }
        }

        public Task TriggerAsync<TEventData>(TEventData eventData)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        Trigger(eventData);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex.Message, ex);
                    }
                });
        }

        #endregion

        #endregion

        #region Private methods

        private static List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            List<IEventHandlerFactory> handlers;
            if (!HandlerFactories.TryGetValue(eventType, out handlers))
            {
                HandlerFactories[eventType] = handlers = new List<IEventHandlerFactory>();
            }

            return handlers;
        }

        #endregion
    }
}