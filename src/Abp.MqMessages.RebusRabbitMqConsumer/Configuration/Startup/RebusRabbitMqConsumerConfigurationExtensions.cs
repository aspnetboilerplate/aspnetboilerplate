using Abp.MqMessages.Consumers;

namespace Abp.Configuration.Startup
{
    public static class RebusRabbitMqConsumerConfigurationExtensions
    {
        public static IRebusRabbitMqConsumerModuleConfig UseRebusRabbitMqConsumer(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.RebusRabbitMqConsumer", () => configurations.AbpConfiguration.IocManager.Resolve<IRebusRabbitMqConsumerModuleConfig>());
        }
    }
}
