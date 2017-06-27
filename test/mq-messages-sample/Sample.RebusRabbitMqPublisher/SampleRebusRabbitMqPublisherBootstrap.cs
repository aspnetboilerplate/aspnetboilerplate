using System.Configuration;
using Abp;
using NLog;
using NLog.Config;

namespace Sample
{
    public class SampleRebusRabbitMqPublisherBootstrap
    {
        private static readonly AbpBootstrapper _bs = AbpBootstrapper.Create<SampleRebusRabbitMqPublisherModule>();

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
