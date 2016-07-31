
using System;

using log4net;
using Castle.Core.Logging;
using log4net.Config;

namespace Abp
{
    public class Log4NetFactory
        : AbstractLoggerFactory
    {
        internal const string DefaultConfigFileName = "log4net.config";

        public Log4NetFactory()
            : this(DefaultConfigFileName)
        {
        }

        public Log4NetFactory(string configFile)
        {
            var file = GetConfigFile(configFile);
            XmlConfigurator.ConfigureAndWatch(file);
        }

        public override ILogger Create(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            var log = LogManager.GetLogger(name);
            return new Log4NetLogger(log, this);
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}