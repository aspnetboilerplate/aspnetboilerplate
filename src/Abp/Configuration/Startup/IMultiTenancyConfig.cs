using Abp.Dependency;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Used to configure multi-tenancy.
    /// </summary>
    public interface IMultiTenancyConfig : ITransientDependency
    {
        /// <summary>
        /// Is multi-tenancy enabled?
        /// Default value: false.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Can host users perform tenant operations
        /// Default value: false.
        /// </summary>
        bool GrantTenantAccessToHostUsers { get; set; }
    }
}