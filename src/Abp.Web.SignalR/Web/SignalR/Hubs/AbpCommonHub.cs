using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.RealTime;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNet.SignalR;

namespace Abp.Web.SignalR.Hubs
{
    public class AbpCommonHub : Hub, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public IAbpSession AbpSession { get; set; }

        private readonly OnlineClientManager _onlineClientManager;

        public AbpCommonHub(OnlineClientManager onlineClientManager)
        {
            _onlineClientManager = onlineClientManager;

            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public async override Task OnConnected()
        {
            await base.OnConnected();

            Logger.Debug("OnConnected: " + Context.ConnectionId);
            Logger.Debug("AbpSession.UserId: " + AbpSession.UserId + ", AbpSession.TenantId: " + AbpSession.TenantId);

            var client = new OnlineClient(
                Context.ConnectionId,
                GetIpAddressOfClient(),
                AbpSession.TenantId,
                AbpSession.UserId
                );

            client["Abp.SignalRClient"] = Clients.Caller;

            _onlineClientManager.Add(client);
        }

        public async override Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);

            try
            {
                Logger.Debug("OnDisconnected: " + Context.ConnectionId);
                _onlineClientManager.Remove(Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
        }

        public async override Task OnReconnected() //TODO: This is for test, remove later.
        {
            await base.OnReconnected();
            Logger.Debug("OnReconnected: " + Context.ConnectionId);
        }

        private string GetIpAddressOfClient()
        {
            try
            {
                return Context.Request.Environment["server.RemoteIpAddress"].ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Can not find IP address of the client! connectionId: " + Context.ConnectionId);
                Logger.Error(ex.Message, ex);
                return "";
            }
        }
    }
}
