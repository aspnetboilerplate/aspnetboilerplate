using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;

namespace Abp.MqMessages.Handlers
{
    /// <summary>
    /// Handle EventData and Publish It into Message Queue.
    /// </summary>
    /// <typeparam name="TEventData">Abp EventData</typeparam>
    /// <typeparam name="TMqMessage">MqMessage（some thing like DTO）</typeparam>
    public abstract class EventDataPublishHandlerBase<TEventData, TMqMessage>
        : IEventHandler<TEventData>, ITransientDependency
        where TEventData : EventData
        where TMqMessage : class
    {
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        public ILogger Logger { get; set; }

        public IMqMessagePublisher MqMessagePublisher { get; set; }

        public EventDataPublishHandlerBase(IUnitOfWorkManager unitOfWorkManager)
        {
            UnitOfWorkManager = unitOfWorkManager;
            Logger = NullLogger.Instance;
            MqMessagePublisher = NullMqMessagePublisher.Instance;
        }

        public virtual void HandleEvent(TEventData eventData)
        {
            if (UnitOfWorkManager.Current == null)
            {
                MqMessagePublisher.Publish(ConvertEventDataToMqMessage(eventData));
            }
            else
            {
                //We should not notify to outside when some thing maybe wrong and uow will rollback. So only when uow Completed, then do it.
                UnitOfWorkManager.Current.Completed += (sender, e) => MqMessagePublisher.Publish(ConvertEventDataToMqMessage(eventData));
            }
        }

        /// <summary>
        /// Map EventData into MqMessage use AutoMapper by default.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public virtual TMqMessage ConvertEventDataToMqMessage(TEventData eventData)
        {
            return eventData.MapTo<TMqMessage>();
        }
    }
}
