
namespace Abp.Events.Producer
{
    public interface IProducer
    {
        void Publish(object events);
    }
}
