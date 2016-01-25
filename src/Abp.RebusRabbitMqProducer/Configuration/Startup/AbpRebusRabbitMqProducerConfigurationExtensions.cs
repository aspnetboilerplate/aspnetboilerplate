using Abp.Events.Producer.RebusRabbitMq;

namespace Abp.Configuration.Startup
{
    public static class AbpRebusRabbitMqProducerConfigurationExtensions
    {
        public static IAbpRebusRabbitMqProducerModuleConfig AbpRebusRabbitMqProducer(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.RebusRabbitMqProducer", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpRebusRabbitMqProducerModuleConfig>());
        }
    }
}
