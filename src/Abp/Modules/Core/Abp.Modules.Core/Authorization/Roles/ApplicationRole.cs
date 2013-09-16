namespace Abp.Modules.Core.Authorization.Roles
{
    /// <summary>
    /// Base class for static roles of an application.
    /// </summary>
    public abstract class ApplicationRole
    {
        /// <summary>
        /// Unique name of this role.
        /// </summary>
        public virtual string Name { get { return GetType().Name; } }

        /// <summary>
        /// Display name of this role.
        /// </summary>
        public virtual string DisplayName { get { return GetType().Name; } }
    }
}
