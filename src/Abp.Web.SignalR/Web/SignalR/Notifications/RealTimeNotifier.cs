using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Notifications;
using Abp.RealTime;
using Abp.Web.SignalR.Hubs;
using Castle.Core.Logging;
using Microsoft.AspNet.SignalR;

namespace Abp.Web.SignalR.Notifications
{
    public class RealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IOnlineClientManager _onlineClientManager;

        private static IHubContext CommonHub
        {
            get
            {
                return GlobalHost.ConnectionManager.GetHubContext<AbpCommonHub>();
            }
        }

        public RealTimeNotifier(IOnlineClientManager onlineClientManager)
        {
            _onlineClientManager = onlineClientManager;
            Logger = NullLogger.Instance;
        }

        public Task SendNotificationAsync(NotificationInfo notification, List<UserNotificationInfo> userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                try
                {
                    var onlineClient = _onlineClientManager.GetByUserIdOrNull(userNotification.UserId);
                    if (onlineClient == null)
                    {
                        //User is not online. No problem, go to the next user.
                        continue;
                    }

                    var signalRClient = CommonHub.Clients.Client(onlineClient.ConnectionId);
                    if (signalRClient == null)
                    {
                        Logger.Debug("Can not get user " + userNotification.UserId + " from SignalR hub!");
                        continue;
                    }

                    signalRClient.getNotification(new
                    {
                        subject = notification.NotificationName,
                        body = "test body for " + notification.NotificationName
                    });
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.ToString(), ex);
                }
            }

            return Task.FromResult(0);
        }
    }
}