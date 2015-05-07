namespace Abp.Auditing
{
    internal class MvcControllersAuditingConfiguration : IMvcControllersAuditingConfiguration
    {
        public bool IsEnabled { get; set; }
    }
}