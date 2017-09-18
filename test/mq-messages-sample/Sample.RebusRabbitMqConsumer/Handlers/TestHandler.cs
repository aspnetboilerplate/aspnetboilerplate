using System.Threading.Tasks;
using Castle.Core.Logging;
using Rebus.Handlers;
using Sample.MqMessages;
using Abp.MqMessages;

namespace Sample.Handlers
{
    public class TestHandler : IHandleMessages<TestMqMessage>
    {
        //As Castle.Windsor-Nlog for Castle.Core 4.1.1 not ready, Now comment it.
        //public ILogger Logger { get; set; }
        public IMqMessagePublisher Publisher { get; set; }
        public TestHandler()
        {
            Publisher = NullMqMessagePublisher.Instance;
        }

        public async Task Handle(TestMqMessage message)
        {
            var msg = $"{message.Name},{message.Value},{message.Time}";
            //Logger.Debug(msg);
            await Publisher.PublishAsync(msg);//send it again ( as a string)!
        }
    }
}
