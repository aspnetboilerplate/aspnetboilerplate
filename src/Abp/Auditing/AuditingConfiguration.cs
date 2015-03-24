namespace Abp.Auditing
{
    internal class AuditingConfiguration : IAuditingConfiguration
    {
        public bool IsEnabled { get; set; }

        public IAuditingSelectorList Selectors { get; private set; }

        public AuditingConfiguration()
        {
            IsEnabled = true;
            Selectors = new AuditingSelectorList();
        }
    }
}