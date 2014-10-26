using Abp.Localization;

namespace Abp.Authorization
{
    /// <summary>
    /// This context is used on <see cref="PermissionProvider.DefinePermissions"/> method.
    /// </summary>
    public interface IPermissionDefinitionContext
    {
        /// <summary>
        /// Creates a root permission group.
        /// </summary>
        /// <param name="name">Unique name of the group</param>
        /// <param name="displayName">Display name of the group</param>
        /// <returns>Created permission group object</returns>
        PermissionGroup CreateRootGroup(string name, ILocalizableString displayName);
        
        /// <summary>
        /// Gets an existing root permission group or null if it does not exists.
        /// </summary>
        /// <param name="name">Unique name of the group</param>
        PermissionGroup GetRootGroupOrNull(string name);
    }
}