namespace Abp.Zero.Configuration
{
    /// <summary>
    /// Configuration options for zero module.
    /// </summary>
    public interface IAbpZeroConfig
    {
        /// <summary>
        /// Gets role management config.
        /// </summary>
        IRoleManagementConfig RoleManagement { get; }

        /// <summary>
        /// Gets user management config.
        /// </summary>
        IUserManagementConfig UserManagement { get; }
    }
}