using Castle.Core.Logging;
using System;

namespace Abp.Castle.Logging.NLog
{
    public class NLogLoggerFactory :  AbstractLoggerFactory
    {
        internal const string DefaultConfigFileName = "nlog.config";
        public NLogLoggerFactory()
            : this(DefaultConfigFileName)
        {
        }

        public NLogLoggerFactory(string configFile)
        {
            var file = GetConfigFile(configFile);
            global::NLog.LogManager.Configuration = new global::NLog.Config.XmlLoggingConfiguration(file.FullName);
        }

        public override ILogger Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new NLogLogger(global::NLog.LogManager.GetLogger(name), this);
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}
