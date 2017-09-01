using Abp.MultiTenancy;

namespace Abp.Zero.Configuration
{
    public class StaticRoleDefinition
    {
        public string RoleName { get; private set; }

        public MultiTenancySides Side { get; private set; }

        public StaticRoleDefinition(string roleName, MultiTenancySides side)
        {
            RoleName = roleName;
            Side = side;
        }
    }
}