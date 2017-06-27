using System;
using Rebus.Config;

namespace Abp.MqMessages.Publishers
{
    public class RebusRabbitMqPublisherModuleConfig : IRebusRabbitMqPublisherModuleConfig
    {
        public RebusRabbitMqPublisherModuleConfig()
        {
            Enabled = true;
            MessageAuditingEnabled = false;
        }
        public string ConnectionString { get; private set; }

        public bool Enabled { get; private set; }

        public Action<RebusLoggingConfigurer> LoggingConfigurer { get; private set; }

        public bool MessageAuditingEnabled { get; private set; }

        public string MessageAuditingQueueName { get; private set; }

        public IRebusRabbitMqPublisherModuleConfig ConnectionTo(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }

        public IRebusRabbitMqPublisherModuleConfig Enable(bool enabled)
        {
            Enabled = enabled;
            return this;
        }

        public IRebusRabbitMqPublisherModuleConfig EnableMessageAuditing(string messageAuditingQueueName)
        {
            MessageAuditingEnabled = true;
            MessageAuditingQueueName = messageAuditingQueueName;
            return this;
        }

        public IRebusRabbitMqPublisherModuleConfig UseLogging(Action<RebusLoggingConfigurer> loggingConfigurer)
        {
            LoggingConfigurer = loggingConfigurer;
            return this;
        }
    }
}
