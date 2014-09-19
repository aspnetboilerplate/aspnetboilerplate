namespace Abp.Authorization.Permissions
{
    /// <summary>
    /// This is the main interface to define permissions for an application.
    /// Implement it to define permissions for your module.
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// This method is called once on application startup to allow to define permissions.
        /// </summary>
        /// <param name="context">Permission definition context</param>
        void DefinePermissions(IPermissionDefinitionContext context);
    }
}