using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Notifications;
using Microsoft.AspNet.SignalR;

namespace Abp.Web.SignalR
{
    //public class AbpNotificationHub : Hub, ITransientDependency
    //{
    //    public AbpNotificationHub()
    //    {
            
    //    }

    //    public override Task OnConnected()
    //    {
    //        return base.OnConnected();
    //    }

    //    public override Task OnDisconnected(bool stopCalled)
    //    {
    //        return base.OnDisconnected(stopCalled);
    //    }

    //    public override Task OnReconnected()
    //    {
    //        return base.OnReconnected();
    //    }
    //}

    //public class SignalRRealTimeNotifier : IRealTimeNotifier
    //{
    //    public Task SendNotificationAsync(NotificationInfo notification, List<UserNotificationInfo> userNotifications)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
