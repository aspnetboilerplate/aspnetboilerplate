namespace Abp.Security.Roles
{
    /// <summary>
    /// Represents a permission for a role.
    /// Used to grant/deny a permission for a role.
    /// </summary>
    public class RolePermission : PermissionSettingEntity
    {
        /// <summary>
        /// Role Id.
        /// </summary>
        public virtual int RoleId { get; set; }
    }
}
