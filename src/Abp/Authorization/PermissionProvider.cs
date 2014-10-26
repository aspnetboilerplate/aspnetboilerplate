using Abp.Dependency;

namespace Abp.Authorization
{
    /// <summary>
    /// This is the main interface to define permissions for an application.
    /// Implement it to define permissions for your module.
    /// </summary>
    public abstract class PermissionProvider : ISingletonDependency
    {
        /// <summary>
        /// This method is called once on application startup to allow to define permissions.
        /// </summary>
        /// <param name="context">Permission definition context</param>
        public virtual void DefinePermissions(IPermissionDefinitionContext context)
        {
            
        }
    }
}