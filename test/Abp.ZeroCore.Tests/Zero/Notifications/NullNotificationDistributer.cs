using System;
using System.Threading.Tasks;
using Abp.Notifications;

namespace Abp.Zero.Notifications
{
    public class NullNotificationDistributer : INotificationDistributer
    {
        public bool IsDistributeCalled { get; set; }

        public async Task DistributeAsync(Guid notificationId)
        {
            IsDistributeCalled = true;
        }
    }
}