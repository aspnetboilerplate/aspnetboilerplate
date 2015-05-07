namespace Abp.Auditing
{
    /// <summary>
    /// Defines MVC Controller auditing configurations
    /// </summary>
    public interface IMvcControllersAuditingConfiguration
    {
        /// <summary>
        /// Used to enable/disable auditing for MVC controllers.
        /// Default: true.
        /// </summary>
        bool IsEnabled { get; set; }
    }
}