using System;
using Abp.Events.Bus;
using Abp.Events.Bus.Datas;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Adds some extension methods to IEventBus to work with unit of work.
    /// </summary>
    public static class UowEventBusExtensions
    {
        /// <summary>
        /// Triggers an event if current unit of work succeed.
        /// </summary>
        /// <typeparam name="TEventData">Event type</typeparam>
        /// <param name="eventBus">EventBus</param>
        /// <param name="eventData">Related data for the event</param>
        public static void TriggerUow<TEventData>(this IEventBus eventBus, TEventData eventData)
            where TEventData : IEventData
        {
            CheckCurrentUow();
            UnitOfWorkScope.Current.OnSuccess(() => eventBus.Trigger(eventData));
        }

        /// <summary>
        /// Triggers an event if current unit of work succeed.
        /// </summary>
        /// <typeparam name="TEventData">Event type</typeparam>
        /// <param name="eventBus">EventBus</param>
        /// <param name="eventSource">The object which triggers the event</param>
        /// <param name="eventData">Related data for the event</param>
        public static void TriggerUow<TEventData>(this IEventBus eventBus, object eventSource, TEventData eventData) where TEventData : IEventData
        {
            CheckCurrentUow();
            UnitOfWorkScope.Current.OnSuccess(() => eventBus.Trigger(eventSource, eventData));
        }

        /// <summary>
        /// Triggers an event if current unit of work succeed.
        /// </summary>
        /// <param name="eventBus">EventBus</param>
        /// <param name="eventType">Event type</param>
        /// <param name="eventData">Related data for the event</param>
        public static void TriggerUow(this IEventBus eventBus, Type eventType, EventData eventData)
        {
            CheckCurrentUow();
            UnitOfWorkScope.Current.OnSuccess(() => eventBus.Trigger(eventType, eventData));
        }

        /// <summary>
        /// Triggers an event if current unit of work succeed.
        /// </summary>
        /// <param name="eventBus">EventBus</param>
        /// <param name="eventType">Event type</param>
        /// <param name="eventSource">The object which triggers the event</param>
        /// <param name="eventData">Related data for the event</param>
        public static void TriggerUow(this IEventBus eventBus, Type eventType, object eventSource, EventData eventData)
        {
            CheckCurrentUow();
            UnitOfWorkScope.Current.OnSuccess(() => eventBus.Trigger(eventType, eventSource, eventData));
        }

        private static void CheckCurrentUow()
        {
            if (UnitOfWorkScope.Current == null)
            {
                throw new AbpException("There is no active unit of work. UnitOfWorkScope.Current is null!");
            }
        }
    }
}
