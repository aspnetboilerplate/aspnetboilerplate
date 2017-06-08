using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.RealTime;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNet.SignalR;

namespace Abp.Web.SignalR.Hubs
{
    /// <summary>
    /// ABP公共枢纽。
    /// </summary>
    public class AbpCommonHub : Hub, ITransientDependency
    {
        /// <summary>
        /// <seealso cref="ILogger"/>
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// <seealso cref="IAbpSession"/>
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        private readonly IOnlineClientManager _onlineClientManager;

        /// <summary>
        /// 初始化<see cref ="AbpCommonHub"/>类的新实例。
        /// </summary>
        public AbpCommonHub(IOnlineClientManager onlineClientManager)
        {
            _onlineClientManager = onlineClientManager;

            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }
        /// <summary>
        /// 客户端注册
        /// </summary>
        public void Register()
        {
            Logger.Debug("A client is registered: " + Context.ConnectionId);
        }
        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnected()
        {
            await base.OnConnected();

            var client = CreateClientForCurrentConnection();

            Logger.Debug("A client is connected: " + client);
            
            _onlineClientManager.Add(client);
        }
        /// <summary>
        /// 客户端重新连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnReconnected()
        {
            await base.OnReconnected();

            var client = _onlineClientManager.GetByConnectionIdOrNull(Context.ConnectionId);
            if (client == null)
            {
                client = CreateClientForCurrentConnection();
                _onlineClientManager.Add(client);
                Logger.Debug("A client is connected (on reconnected event): " + client);
            }
            else
            {
                Logger.Debug("A client is reconnected: " + client);
            }
        }
        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 为当前连接创建客户端
        /// </summary>
        /// <returns></returns>
        private IOnlineClient CreateClientForCurrentConnection()
        {
            return new OnlineClient(
                Context.ConnectionId,
                GetIpAddressOfClient(),
                AbpSession.TenantId,
                AbpSession.UserId
            );
        }
        /// <summary>
        /// 获取客户端的IP地址
        /// </summary>
        /// <returns></returns>
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
