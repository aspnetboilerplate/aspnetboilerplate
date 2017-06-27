using Abp;

namespace Sample
{
    public class SampleRebusRabbitMqConsumerBootstrap
    {
        private static readonly AbpBootstrapper _bs = AbpBootstrapper.Create<SampleRebusRabbitMqConsumerModule>();

        public void Start()
        {
            //LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");
            _bs.Initialize();
        }

        public void Stop()
        {
            _bs.Dispose();
        }
    }
}
