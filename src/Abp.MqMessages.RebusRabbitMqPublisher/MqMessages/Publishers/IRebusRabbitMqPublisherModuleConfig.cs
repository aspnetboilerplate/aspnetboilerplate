using System;
using Rebus.Config;

namespace Abp.MqMessages.Publishers
{
    public interface IRebusRabbitMqPublisherModuleConfig
    {
        bool Enabled { get; }
        
        Action<RebusLoggingConfigurer> LoggingConfigurer { get; }
        
        string ConnectionString { get; }
        
        bool MessageAuditingEnabled { get; }
        
        string MessageAuditingQueueName { get; }
        
        IRebusRabbitMqPublisherModuleConfig Enable(bool enabled);
        
        IRebusRabbitMqPublisherModuleConfig ConnectionTo(string connectionString);
        
        IRebusRabbitMqPublisherModuleConfig UseLogging(Action<RebusLoggingConfigurer> loggingConfigurer);
        
        IRebusRabbitMqPublisherModuleConfig EnableMessageAuditing(string messageAuditingQueueName);
    }
}
