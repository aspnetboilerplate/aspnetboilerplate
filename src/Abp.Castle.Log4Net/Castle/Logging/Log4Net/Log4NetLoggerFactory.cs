using System;
using Castle.Core.Logging;
using log4net;
using log4net.Config;

namespace Abp.Castle.Logging.Log4Net
{
    public class Log4NetLoggerFactory : AbstractLoggerFactory
    {
        internal const string DefaultConfigFileName = "log4net.config";

        public Log4NetLoggerFactory()
            : this(DefaultConfigFileName)
        {
        }

        public Log4NetLoggerFactory(string configFileName)
        {
            var file = GetConfigFile(configFileName);
            XmlConfigurator.ConfigureAndWatch(file);
        }

        public override ILogger Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new Log4NetLogger(LogManager.GetLogger(name), this);
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}