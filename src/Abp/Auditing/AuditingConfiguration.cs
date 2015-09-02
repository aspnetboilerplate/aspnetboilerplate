namespace Abp.Auditing
{
    internal class AuditingConfiguration : IAuditingConfiguration
    {
        public bool IsEnabled { get; set; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public IMvcControllersAuditingConfiguration MvcControllers { get; private set; }

        public IAuditingSelectorList Selectors { get; private set; }

        public AuditingConfiguration()
        {
            IsEnabled = true;
            Selectors = new AuditingSelectorList();
            MvcControllers = new MvcControllersAuditingConfiguration();
        }
    }
}