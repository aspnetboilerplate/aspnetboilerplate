namespace Abp.Zero.SampleApp.Application
{
    public class AppPermissions
    {
        public const string WebHookPermission = "WebHook";

        public class WebHook
        {
            public const string TenantDeleted = "WebHook.TenantDeleted";
        }
    }
}
