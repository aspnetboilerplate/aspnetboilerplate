namespace Abp.Zero.SampleApp.Application
{
    public class AppPermissions
    {
        public const string WebHookMainPermission = "Abp.Zero.Webhooks";

        public class WebHook
        {
            public const string TenantMainPermission = "Abp.Zero.Webhooks.Tenant";

            public class Tenant
            {
                public const string TenantDeleted = "Abp.Zero.Webhooks.TenantDeleted";
            }

            public const string UserCreated = "Abp.Zero.Webhooks.UserCreated";
        }
    }
}
