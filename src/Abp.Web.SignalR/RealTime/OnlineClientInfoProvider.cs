using System;
using Abp.Auditing;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNet.SignalR.Hubs;

namespace Abp.RealTime
{
    public class OnlineClientInfoProvider : IOnlineClientInfoProvider
    {
        private readonly IClientInfoProvider _clientInfoProvider;

        public IAbpSession AbpSession { get; set; }
        public ILogger Logger { get; set; }

        public OnlineClientInfoProvider(IClientInfoProvider clientInfoProvider)
        {
            _clientInfoProvider = clientInfoProvider;
            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
        }

        public IOnlineClient CreateClientForCurrentConnection(HubCallerContext context)
        {
            return new OnlineClient(
                context.ConnectionId,
                GetIpAddressOfClient(context),
                AbpSession.TenantId,
                AbpSession.UserId
            );
        }

        private string GetIpAddressOfClient(HubCallerContext context)
        {
            try
            {
                return _clientInfoProvider.ClientIpAddress;
            }
            catch (Exception ex)
            {
                Logger.Error("Can not find IP address of the client! connectionId: " + context.ConnectionId);
                Logger.Error(ex.Message, ex);
                return "";
            }
        }
    }
}
