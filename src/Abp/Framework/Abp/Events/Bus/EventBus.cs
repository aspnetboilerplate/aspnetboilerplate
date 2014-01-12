using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Events.Bus.Datas;
using Abp.Events.Bus.Datas.Entities;
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
        #region Public properties

        /// <summary>
        /// Gets the default <see cref="EventBus"/> instance.
        /// </summary>
        public static EventBus Default { get { return DefaultInstance; } }
        private static readonly EventBus DefaultInstance = new EventBus();

        /// <summary>
        /// Reference to the Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        #endregion

        #region Private fields

        /// <summary>
        /// All registered handler factories.
        /// </summary>
        private readonly Dictionary<Type, List<IEventHandlerFactory>> _handlerFactories;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="EventBus"/> instance.
        /// Instead of creating a new instace, you can use <see cref="Default"/> to use Global <see cref="EventBus"/>.
        /// </summary>
        public EventBus()
        {
            _handlerFactories = new Dictionary<Type, List<IEventHandlerFactory>>();
            Logger = NullLogger.Instance;
        }

        #endregion

        #region Public methods

        #region Register

        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return Register(typeof(TEventData), new ActionEventHandler<TEventData>(action));
        }

        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return Register(typeof(TEventData), handler);
        }

        public IDisposable Register<TEventData, THandler>() where TEventData : IEventData where THandler : IEventHandler<TEventData>, new()
        {
            return Register(typeof (TEventData), new TransientEventHandlerFactory<THandler>());
        }

        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return Register(eventType, new SingleInstanceHandlerFactory(handler));
        }

        public IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData
        {
            return Register(typeof(TEventData), handlerFactory);
        }

        public IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory)
        {
            lock (_handlerFactories)
            {
                GetOrCreateHandlerFactories(eventType).Add(handlerFactory);
                return new FactoryUnregisterer(this, eventType, handlerFactory);
            }
        }

        #endregion

        #region Unregister

        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            lock (_handlerFactories)
            {
                GetOrCreateHandlerFactories(typeof(TEventData))
                    .RemoveAll(
                        factory =>
                        {
                            if (factory is SingleInstanceHandlerFactory)
                            {
                                var singleInstanceFactoru = factory as SingleInstanceHandlerFactory;
                                if (singleInstanceFactoru.HandlerInstance is ActionEventHandler<TEventData>)
                                {
                                    var actionHandler = singleInstanceFactoru.HandlerInstance as ActionEventHandler<TEventData>;
                                    if (actionHandler.Action == action)
                                    {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        });
            }
        }

        public void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            Unregister(typeof(TEventData), handler);
        }

        public void Unregister(Type eventType, IEventHandler handler)
        {
            lock (_handlerFactories)
            {
                GetOrCreateHandlerFactories(eventType)
                    .RemoveAll(
                        factory =>
                        factory is SingleInstanceHandlerFactory && (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                    );
            }
        }

        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            Unregister(typeof(TEventData), factory);
        }

        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
            lock (_handlerFactories)
            {
                GetOrCreateHandlerFactories(eventType).Remove(factory);
            }
        }

        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
            UnregisterAll(typeof(TEventData));
        }

        public void UnregisterAll(Type eventType)
        {
            lock (_handlerFactories)
            {
                GetOrCreateHandlerFactories(eventType).Clear();
            }
        }

        #endregion

        #region Trigger

        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            Trigger(null, eventData);
        }

        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            eventData.EventSource = eventSource;
            IEventHandlerFactory[] factoriesToTrigger;
            lock (_handlerFactories)
            {
                List<IEventHandlerFactory> handlerFactoryList;
                if (!_handlerFactories.TryGetValue(typeof(TEventData), out handlerFactoryList))
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

        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return TriggerAsync(null, eventData);
        }

        public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            return Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        Trigger(eventSource, eventData);
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

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            List<IEventHandlerFactory> handlers;
            if (!_handlerFactories.TryGetValue(eventType, out handlers))
            {
                _handlerFactories[eventType] = handlers = new List<IEventHandlerFactory>();
            }

            return handlers;
        }

        #endregion
    }
}