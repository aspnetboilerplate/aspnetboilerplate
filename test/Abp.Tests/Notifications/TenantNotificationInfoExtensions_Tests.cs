using Abp.Notifications;
using Shouldly;
using Xunit;

namespace Abp.Tests.Notifications
{
    public class TenantNotificationInfoExtensions_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ToTenantNotification_Test()
        {
            var tenantNotification = new TenantNotificationInfo()
            {
                Data = @"{""Message"":""This is a test notification, created at 20.06.2022 14:14:57"",""Type"":""Abp.Notifications.MessageNotificationData"",""Properties"":{""Message"":""This is a test notification, created at 20.06.2022 14:14:57""}}",
                DataTypeName = "Abp.Notifications.MessageNotificationData, Abp, Version=7.3.0.0, Culture=neutral, PublicKeyToken=null"
            }.ToTenantNotification();
            tenantNotification.Data.ShouldNotBeNull();
        }
    }
}