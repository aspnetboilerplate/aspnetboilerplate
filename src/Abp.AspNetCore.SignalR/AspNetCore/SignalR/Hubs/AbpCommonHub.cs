using Abp.Auditing;
using Abp.RealTime;

namespace Abp.AspNetCore.SignalR.Hubs
{
    public class AbpCommonHub : OnlineClientHubBase
    {
        public AbpCommonHub(IOnlineClientManager onlineClientManager, IClientInfoProvider clientInfoProvider) 
            : base(onlineClientManager, clientInfoProvider)
        {
        }

        public void Register()
        {
            Logger.Debug("A client is registered: " + Context.ConnectionId);
        }
    }
}
