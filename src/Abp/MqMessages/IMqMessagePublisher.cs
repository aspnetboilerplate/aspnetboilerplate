using System.Threading.Tasks;

namespace Abp.MqMessages
{
    public interface IMqMessagePublisher
    {
        void Publish(object mqMessages);

        Task PublishAsync(object mqMessages);
    }
}
