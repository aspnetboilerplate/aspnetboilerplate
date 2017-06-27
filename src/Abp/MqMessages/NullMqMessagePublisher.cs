using System.Threading.Tasks;

namespace Abp.MqMessages
{
    public class NullMqMessagePublisher : IMqMessagePublisher
    {
        public static NullMqMessagePublisher Instance { get { return SingletonInstance; } }

        private static readonly NullMqMessagePublisher SingletonInstance = new NullMqMessagePublisher();
        
        public Task PublishAsync(object mqMessages)
        {
            //do nothing.
            return Task.FromResult(0);
        }
        
        public void Publish(object mqMessages)
        {
            //do nothing.
        }
    }
}
