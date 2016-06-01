using Abp.Dependency;
namespace Abp.Events.Producer.RebusRabbitMq
{
    internal class AbpRebusRabbitMqProducerModuleConfig : IAbpRebusRabbitMqProducerModuleConfig
    {
        public string ConnectionString { get; set; }
    }
}
