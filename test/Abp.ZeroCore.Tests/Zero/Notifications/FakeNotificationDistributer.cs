using System;
using System.Threading.Tasks;
using Abp.Notifications;

namespace Abp.Zero.Notifications
{
    public class FakeNotificationDistributer : INotificationDistributer
    {
        public bool IsDistributeCalled { get; set; }

        public async Task DistributeAsync(Guid notificationId)
        {
            IsDistributeCalled = true;
        }

        public void Distribute(Guid notificationId)
        {
            IsDistributeCalled = true;
        }
    }
}