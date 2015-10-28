using Abp.Authorization.Roles;
using MyAbpZeroProject.MultiTenancy;
using MyAbpZeroProject.Users;

namespace MyAbpZeroProject.Authorization.Roles
{
    public class Role : AbpRole<Tenant, User>
    {

    }
}