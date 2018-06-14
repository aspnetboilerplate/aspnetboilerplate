using System;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Dependency;
using Abp.RealTime;
using Abp.Runtime.Session;
using Castle.Core.Logging;

namespace Abp.AspNetCore.SignalR.Hubs
{
    public abstract class OnlineClientHubBase : AbpHubBase, ITransientDependency
    {
        protected IOnlineClientManager OnlineClientManager { get; }
        protected IClientInfoProvider ClientInfoProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpCommonHub"/> class.
        /// </summary>
        protected OnlineClientHubBase(
            IOnlineClientManager onlineClientManager,
            IClientInfoProvider clientInfoProvider)
        {
            OnlineClientManager = onlineClientManager;
            ClientInfoProvider = clientInfoProvider;

            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var client = CreateClientForCurrentConnection();

            Logger.Debug("A client is connected: " + client);

            OnlineClientManager.Add(client);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            Logger.Debug("A client is disconnected: " + Context.ConnectionId);

            try
            {
                OnlineClientManager.Remove(Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
        }

        protected virtual IOnlineClient CreateClientForCurrentConnection()
        {
            return new OnlineClient(
                Context.ConnectionId,
                GetIpAddressOfClient(),
                AbpSession.TenantId,
                AbpSession.UserId
            );
        }

        protected virtual string GetIpAddressOfClient()
        {
            try
            {
                return ClientInfoProvider.ClientIpAddress;
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