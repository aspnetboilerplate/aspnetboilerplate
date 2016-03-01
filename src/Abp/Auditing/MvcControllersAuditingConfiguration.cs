namespace Adorable.Auditing
{
    internal class MvcControllersAuditingConfiguration : IMvcControllersAuditingConfiguration
    {
        public bool IsEnabled { get; set; }

        public bool IsEnabledForChildActions { get; set; }

        public MvcControllersAuditingConfiguration()
        {
            IsEnabled = true;
        }
    }
}