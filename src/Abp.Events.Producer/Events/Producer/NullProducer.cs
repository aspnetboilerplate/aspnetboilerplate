
namespace Abp.Events.Producer
{
    public class NullProducer : IProducer
    {
        public static NullProducer Instance { get { return SingletonInstance; } }
        private static readonly NullProducer SingletonInstance = new NullProducer();

        public void Publish(object events)
        {
        }
    }
}
