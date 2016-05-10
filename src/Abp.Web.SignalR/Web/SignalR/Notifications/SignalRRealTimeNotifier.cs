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
    ///     Implements <see cref="IRealTimeNotifier" /> to send notifications via SignalR.
    /// </summary>
    public class SignalRRealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        private readonly IOnlineClientManager _onlineClientManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SignalRRealTimeNotifier" /> class.
        /// </summary>
        public SignalRRealTimeNotifier(IOnlineClientManager onlineClientManager)
        {
            _onlineClientManager = onlineClientManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        ///     Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        private static IHubContext CommonHub
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<AbpCommonHub>(); }
        }

        /// <inheritdoc />
        public Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                try
                {
                    var onlineClient = _onlineClientManager.GetByUserIdOrNull(userNotification);
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

                    //TODO: await call or not?
                    signalRClient.getNotification(userNotification);
                }
                catch (Exception ex)
                {
                    Logger.Warn("Could not send notification to userId: " + userNotification.UserId);
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            return Task.FromResult(0);
        }
    }
}