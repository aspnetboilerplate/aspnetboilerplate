namespace Abp.Modules.Core.Authorization.Roles
{
    public class ApplicationUser : ApplicationRole
    {
        public override string Name { get { return BasicRoles.ApplicationUser; } }

        public override string DisplayName { get { return BasicRoles.ApplicationUser; } }
    }
}