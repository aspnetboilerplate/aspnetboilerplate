namespace Abp.Auditing
{
    internal class AuditingConfiguration : IAuditingConfiguration
    {
        public AuditingConfiguration()
        {
            IsEnabled = true;
            Selectors = new AuditingSelectorList();
            MvcControllers = new MvcControllersAuditingConfiguration();
        }

        public bool IsEnabled { get; set; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public IMvcControllersAuditingConfiguration MvcControllers { get; }

        public IAuditingSelectorList Selectors { get; }
    }
}