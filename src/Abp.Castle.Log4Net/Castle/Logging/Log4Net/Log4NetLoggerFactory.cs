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
            var file = GetConfigFile(configFileName);

            _loggerRepository = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy)
            );

            XmlConfigurator.ConfigureAndWatch(_loggerRepository, file);

            //TODO: Test this code since it's changed on netstandard2.0 change
        }

        public override ILogger Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new Log4NetLogger(LogManager.GetLogger(_loggerRepository.Name, name), this);
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}