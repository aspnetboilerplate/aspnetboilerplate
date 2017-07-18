using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Castle.Core.Logging;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Abp.Castle.Logging.Log4Net
{
    public class Log4NetLoggerFactory : AbstractLoggerFactory
    {
        internal const string DefaultConfigFileName = "log4net.config";
        private readonly ILoggerRepository _loggerRepository;

        public Log4NetLoggerFactory()
            : this(DefaultConfigFileName)
        {
        }

        public Log4NetLoggerFactory(string configFileName)
        {
#if NET46
            var file = GetConfigFile(configFileName);
            XmlConfigurator.ConfigureAndWatch(file);
#else
            _loggerRepository = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy)
            );

            var log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead(configFileName));
            XmlConfigurator.Configure(_loggerRepository, log4NetConfig["log4net"]);
#endif
        }

        public override ILogger Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

#if NET46
            return new Log4NetLogger(LogManager.GetLogger(name), this);
#else
            return new Log4NetLogger(LogManager.GetLogger(_loggerRepository.Name, name), this);
#endif
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}