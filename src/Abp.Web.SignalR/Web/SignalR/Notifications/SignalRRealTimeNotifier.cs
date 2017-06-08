using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Notifications;
using Abp.RealTime;
using Abp.Web.SignalR.Hubs;
using Castle.Core.Logging;
using Microsoft.AspNet.SignalR;

namespace Abp.Web.SignalR.Notifications
{
    /// <summary>
    /// 实现<see cref ="IRealTimeNotifier"/>通过SignalR发送通知。
    /// </summary>
    public class SignalRRealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        /// <summary>
        /// <seealso cref="ILogger"/>
        /// </summary>
        public ILogger Logger { get; set; }

        private readonly IOnlineClientManager _onlineClientManager;

        private static IHubContext CommonHub
        {
            get
            {
                return GlobalHost.ConnectionManager.GetHubContext<AbpCommonHub>();
            }
        }

        /// <summary>
        /// 初始化<see cref ="SignalRRealTimeNotifier"/>类的新实例。
        /// </summary>
        public SignalRRealTimeNotifier(IOnlineClientManager onlineClientManager)
        {
            _onlineClientManager = onlineClientManager;
            Logger = NullLogger.Instance;
        }

        /// <inheritdoc/>
        public Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                try
                {
                    var onlineClients = _onlineClientManager.GetAllByUserId(userNotification);
                    foreach (var onlineClient in onlineClients)
                    {
                        var signalRClient = CommonHub.Clients.Client(onlineClient.ConnectionId);
                        if (signalRClient == null)
                        {
                            Logger.Debug("Can not get user " + userNotification.ToUserIdentifier() + " with connectionId " + onlineClient.ConnectionId + " from SignalR hub!");
                            continue;
                        }

                        signalRClient.getNotification(userNotification);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Could not send notification to user: " + userNotification.ToUserIdentifier());
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            return Task.FromResult(0);
        }
    }
}