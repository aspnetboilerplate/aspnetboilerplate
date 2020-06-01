using System;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Auditing;
using Microsoft.AspNetCore.SignalR;

namespace Abp.RealTime
{
    public class OnlineClientInfoProvider : AbpServiceBase, IOnlineClientInfoProvider
    {

        private readonly IClientInfoProvider _clientInfoProvider;

        public OnlineClientInfoProvider(IClientInfoProvider clientInfoProvider)
        {
            _clientInfoProvider = clientInfoProvider;
        }

        public IOnlineClient CreateClientForCurrentConnection(HubCallerContext context)
        {
            return new OnlineClient(
                context.ConnectionId,
                GetIpAddressOfClient(context),
                context.GetTenantId(),
                context.GetUserIdOrNull()
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
