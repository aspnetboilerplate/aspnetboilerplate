using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.RealTime;
using Abp.Runtime.Session;
using Castle.Core.Logging;

namespace Abp.Web.SignalR.Hubs
{
    /// <summary>
    /// Common Hub of ABP.
    /// </summary>
    public class AbpCommonHub : AbpHubBase, ITransientDependency
    {
        private readonly IOnlineClientManager _onlineClientManager;
        private readonly IOnlineClientInfoProvider _onlineClientInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpCommonHub"/> class.
        /// </summary>
        public AbpCommonHub(
            IOnlineClientManager onlineClientManager,
            IOnlineClientInfoProvider onlineClientInfoProvider)
        {
            _onlineClientManager = onlineClientManager;
            _onlineClientInfoProvider = onlineClientInfoProvider;

            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public void Register()
        {
            Logger.Debug("A client is registered: " + Context.ConnectionId);
        }

        public override async Task OnConnected()
        {
            await base.OnConnected();

            var client = _onlineClientInfoProvider.CreateClientForCurrentConnection(Context);

            Logger.Debug("A client is connected: " + client);

            _onlineClientManager.Add(client);
        }

        public override async Task OnReconnected()
        {
            await base.OnReconnected();

            var client = _onlineClientManager.GetByConnectionIdOrNull(Context.ConnectionId);
            if (client == null)
            {
                client = _onlineClientInfoProvider.CreateClientForCurrentConnection(Context);
                _onlineClientManager.Add(client);
                Logger.Debug("A client is connected (on reconnected event): " + client);
            }
            else
            {
                Logger.Debug("A client is reconnected: " + client);
            }
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);

            Logger.Debug("A client is disconnected: " + Context.ConnectionId);

            try
            {
                _onlineClientManager.Remove(Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
        }
    }
}
