
namespace Abp.Events.Producer.RebusRabbitMq
{
    public interface IAbpRebusRabbitMqProducerModuleConfig
    {
        string ConnectionString { get; set; }
    }
}
