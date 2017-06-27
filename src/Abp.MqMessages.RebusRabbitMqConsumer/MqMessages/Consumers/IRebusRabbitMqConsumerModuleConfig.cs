using System;
using System.Reflection;
using Rebus.Config;
using Rebus.Serialization;

namespace Abp.MqMessages.Consumers
{
    public interface IRebusRabbitMqConsumerModuleConfig
    {
        bool Enabled { get; }
        
        string ConnectionString { get; }
        
        string QueueName { get; }
        
        int MaxParallelism { get; }
        
        int NumberOfWorkers { get; }
        
        bool MessageAuditingEnabled { get; }
        
        string MessageAuditingQueueName { get; }
        
        Assembly[] AssemblysIncludeRebusMqMessageHandlers { get; }
        
        Action<RebusLoggingConfigurer> LoggingConfigurer { get; }
        
        Action<OptionsConfigurer> OptionsConfigurer { get; }
        
        Action<StandardConfigurer<ISerializer>> SerializerConfigurer { get; }
        
        IRebusRabbitMqConsumerModuleConfig Enable(bool enabled);
        
        IRebusRabbitMqConsumerModuleConfig ConnectTo(string connectString);
        
        IRebusRabbitMqConsumerModuleConfig UseQueue(string queueName);
        
        IRebusRabbitMqConsumerModuleConfig SetMaxParallelism(int maxParallelism);
        
        IRebusRabbitMqConsumerModuleConfig SetNumberOfWorkers(int numberOfWorkers);
        
        IRebusRabbitMqConsumerModuleConfig EnableMessageAuditing(string messageAuditingQueueName);
        
        IRebusRabbitMqConsumerModuleConfig RegisterHandlerInAssemblys(params Assembly[] assemblys);
        
        IRebusRabbitMqConsumerModuleConfig UseLogging(Action<RebusLoggingConfigurer> loggingConfigurer);
        
        IRebusRabbitMqConsumerModuleConfig UseOptions(Action<OptionsConfigurer> optionsConfigurer);
        
        IRebusRabbitMqConsumerModuleConfig UseSerializer(Action<StandardConfigurer<ISerializer>> serializerConfigurer);
    }
}
