using Abp.Zero.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Users;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class RoleMap : AbpRoleMap<Role, User>
    {
        
    }
}