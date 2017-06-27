using System.Reflection;
using System.Threading.Tasks;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.Threading;
using Castle.Core.Logging;
using Rebus.Bus;

namespace Abp.MqMessages.Publishers
{
    public class RebusRabbitMqPublisher : IMqMessagePublisher
    {
        private readonly IBus _bus;

        public ILogger Logger { get; set; }

        public IAbpSession AbpSession { get; set; }

        public RebusRabbitMqPublisher(IBus bus)
        {
            _bus = bus;
            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public void Publish(object mqMessages)
        {
            TryFillSessionInfo(mqMessages);

            Logger.Debug(mqMessages.GetType().FullName + ":" + mqMessages.ToJsonString());

            AsyncHelper.RunSync(() => _bus.Publish(mqMessages));
        }

        private void TryFillSessionInfo(object mqMessages)
        {
            if (AbpSession.UserId.HasValue)
            {
                var operatorUserIdProperty = mqMessages.GetType().GetProperty("UserId");
                if (operatorUserIdProperty != null && (operatorUserIdProperty.PropertyType == typeof(long?)))
                {
                    operatorUserIdProperty.SetValue(mqMessages, AbpSession.UserId);
                }
            }

            if (AbpSession.TenantId.HasValue)
            {
                var tenantIdProperty = mqMessages.GetType().GetProperty("TenantId");
                if (tenantIdProperty != null && (tenantIdProperty.PropertyType == typeof(int?)))
                {
                    tenantIdProperty.SetValue(mqMessages, AbpSession.TenantId);
                }
            }
        }

        public async Task PublishAsync(object mqMessages)
        {
            TryFillSessionInfo(mqMessages);

            Logger.Debug(mqMessages.GetType().FullName + ":" + mqMessages.ToJsonString());

            await _bus.Publish(mqMessages);
        }
    }
}
