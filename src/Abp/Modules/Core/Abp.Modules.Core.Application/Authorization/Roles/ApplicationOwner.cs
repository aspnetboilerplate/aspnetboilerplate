namespace Abp.Authorization.Roles
{
    /// <summary>
    /// Only one user must has this role and he must be the owner of the tenant application.
    /// </summary>
    public class ApplicationOwner : StaticRole
    {
        public override string Name { get { return BasicRoles.ApplicationOwner; } }

        public override string DisplayName { get { return BasicRoles.ApplicationOwner; } }

        public override bool IsFrozen { get { return true; } }

        public override bool IsSetOnlyInCode { get { return true; } }
    }
}