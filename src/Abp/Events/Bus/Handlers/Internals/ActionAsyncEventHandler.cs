using System;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Events.Bus.Handlers.Internals
{
    /// <summary>
    /// This event handler is an adapter to be able to use an action as <see cref="IEventHandler{TEventData}"/> implementation.
    /// </summary>
    /// <typeparam name="TEventData">Event type</typeparam>
    internal class ActionAsyncEventHandler<TEventData> :
        IAsyncEventHandler<TEventData>,
        ITransientDependency
    {
        /// <summary>
        /// Function to handle the event.
        /// </summary>
        public Func<TEventData, Task> Action { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ActionAsyncEventHandler{TEventData}"/>.
        /// </summary>
        /// <param name="handler">Action to handle the event</param>
        public ActionAsyncEventHandler(Func<TEventData, Task> handler)
        {
            Action = handler;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventData"></param>
        public async Task HandleEventAsync(TEventData eventData)
        {
            await Action(eventData);
        }
    }
}