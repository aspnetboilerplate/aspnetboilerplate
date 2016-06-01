using System.Reflection;
using Abp.Modules;
using Rebus.CastleWindsor;
using Rebus.Config;
using Rebus.RabbitMq;

namespace Abp.Events.Producer.RebusRabbitMq
{
    [DependsOn(typeof(AbpEventsProducerModule))]
    public class AbpRebusRabbitMqProducerModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpRebusRabbitMqProducerModuleConfig, AbpRebusRabbitMqProducerModuleConfig>();
            IocManager.Register<IProducer, RebusRabbitMqProducer>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            var config = IocManager.Resolve<IAbpRebusRabbitMqProducerModuleConfig>();
            var container = IocManager.IocContainer;
            var adapter = new CastleWindsorContainerAdapter(container);

            var bus = Configure.With(adapter)
                .Transport(t => t.UseRabbitMqAsOneWayClient(config.ConnectionString))
                .Start();
        }
    }
}
