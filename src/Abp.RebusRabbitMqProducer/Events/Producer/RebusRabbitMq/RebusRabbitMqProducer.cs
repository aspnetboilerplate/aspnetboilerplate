using Rebus.Bus;

namespace Abp.Events.Producer.RebusRabbitMq
{
    public class RebusRabbitMqProducer : IProducer
    {
        private IBus _bus;

        public RebusRabbitMqProducer(IBus bus)
        {
            _bus = bus;
        }

        public void Publish(object events)
        {
            _bus.Publish(events);
        }
    }
}
