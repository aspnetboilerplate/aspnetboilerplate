namespace Abp.Auditing
{
    /// <summary>
    /// Used to configure auditing.
    /// </summary>
    public interface IAuditingConfiguration
    {
        /// <summary>
        /// Used to enable/disable auditing system.
        /// Default: true. Set false to completely disable it.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Set true to enable saving audit logs if current user is not logged in.
        /// Default: false.
        /// </summary>
        bool IsEnabledForAnonymousUsers { get; set; }

        /// <summary>
        /// Used to configure auditing for MVC Controllers.
        /// </summary>
        IMvcControllersAuditingConfiguration MvcControllers { get; }

        /// <summary>
        /// List of selectors to select classes/interfaces which should be audited as default.
        /// </summary>
        IAuditingSelectorList Selectors { get; }
    }
}