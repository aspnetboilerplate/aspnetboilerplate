namespace Abp.Auditing
{
    internal class MvcControllersAuditingConfiguration : IMvcControllersAuditingConfiguration
    {
        public MvcControllersAuditingConfiguration()
        {
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        public bool IsEnabledForChildActions { get; set; }
    }
}