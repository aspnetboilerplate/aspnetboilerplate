using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Handlers;

namespace Abp.Events.Producer.Handler
{
    public class PublishAllEventsHandler : IEventHandler<EventData>, ITransientDependency
    {
        private readonly IProducer _producer;

        public PublishAllEventsHandler()
        {
            _producer = NullProducer.Instance;
        }

        public PublishAllEventsHandler(IProducer producer)
        {
            _producer = producer;
        }

        public void HandleEvent(EventData eventData)
        {
            if (eventData is IShouldBePublish)
            {
                _producer.Publish(eventData);
            }
        }
    }
}
