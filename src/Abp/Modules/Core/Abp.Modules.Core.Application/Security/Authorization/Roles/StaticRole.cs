namespace Abp.Security.Authorization.Roles
{
    public abstract class StaticRole : ApplicationRole
    {
        /// <summary>
        /// If true, features of this role can not be modified.
        /// </summary>
        public virtual bool IsFrozen { get { return false; } }

        /// <summary>
        /// If true, this role can only be set in code (programmatically).
        /// </summary>
        public virtual bool IsSetOnlyInCode { get { return false; } }
    }
}