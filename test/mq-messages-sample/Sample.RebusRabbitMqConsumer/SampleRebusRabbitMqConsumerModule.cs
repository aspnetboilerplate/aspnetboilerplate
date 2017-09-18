using Abp.Modules;
using Abp.MqMessages.Consumers;
using Abp.Configuration.Startup;
using System.Reflection;
using Rebus.NLog;
using Castle.Facilities.Logging;

namespace Sample
{
    [DependsOn(typeof(RebusRabbitMqConsumerModule))]
    public class SampleRebusRabbitMqConsumerModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.UseRebusRabbitMqConsumer()
                //.UseLogging(c => c.NLog())
                .ConnectTo("amqp://dev:dev@rabbitmq.local.jk724.cn/dev_host")
                .UseQueue(Assembly.GetExecutingAssembly().GetName().Name)
                .RegisterHandlerInAssemblys(Assembly.GetExecutingAssembly());
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void PostInitialize()
        {
            //As Castle.Windsor-Nlog for Castle.Core 4.1.1 not ready, Now comment it.
            // Abp.Dependency.IocManager.Instance.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog().WithConfig("nlog.config"));
        }
    }
}
