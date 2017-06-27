using System;
using System.Reflection;
using Newtonsoft.Json;
using Rebus.Config;
using Rebus.NewtonsoftJson;
using Rebus.Serialization;

namespace Abp.MqMessages.Consumers
{
    public class RebusRabbitMqConsumerModuleConfig : IRebusRabbitMqConsumerModuleConfig
    {
        public RebusRabbitMqConsumerModuleConfig()
        {
            Enabled = true;
            MessageAuditingEnabled = false;
            MaxParallelism = 1;
            NumberOfWorkers = 1;
            SerializerConfigurer = c => c.UseNewtonsoftJson(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }

        public Assembly[] AssemblysIncludeRebusMqMessageHandlers { get; private set; }

        public string ConnectionString { get; private set; }

        public bool Enabled { get; private set; }

        public Action<RebusLoggingConfigurer> LoggingConfigurer { get; private set; }

        public int MaxParallelism { get; private set; }

        public bool MessageAuditingEnabled { get; private set; }

        public string MessageAuditingQueueName { get; private set; }

        public int NumberOfWorkers { get; private set; }

        public Action<OptionsConfigurer> OptionsConfigurer { get; private set; }

        public string QueueName { get; private set; }

        public Action<StandardConfigurer<ISerializer>> SerializerConfigurer { get; private set; }

        public IRebusRabbitMqConsumerModuleConfig ConnectTo(string connectString)
        {
            ConnectionString = connectString;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig Enable(bool enabled)
        {
            Enabled = enabled;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig EnableMessageAuditing(string messageAuditingQueueName)
        {
            MessageAuditingEnabled = true;
            MessageAuditingQueueName = messageAuditingQueueName;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig RegisterHandlerInAssemblys(params Assembly[] assemblys)
        {
            AssemblysIncludeRebusMqMessageHandlers = assemblys;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig SetMaxParallelism(int maxParallelism)
        {
            MaxParallelism = maxParallelism;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig SetNumberOfWorkers(int numberOfWorkers)
        {
            NumberOfWorkers = numberOfWorkers;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig UseLogging(Action<RebusLoggingConfigurer> loggingConfigurer)
        {
            LoggingConfigurer = loggingConfigurer;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig UseOptions(Action<OptionsConfigurer> optionsConfigurer)
        {
            OptionsConfigurer = optionsConfigurer;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig UseQueue(string queueName)
        {
            QueueName = queueName;
            return this;
        }

        public IRebusRabbitMqConsumerModuleConfig UseSerializer(Action<StandardConfigurer<ISerializer>> serializerConfigurer)
        {
            SerializerConfigurer = serializerConfigurer;
            return this;
        }
    }
}
