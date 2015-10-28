using Abp.MultiTenancy;
using MyAbpZeroProject.Users;

namespace MyAbpZeroProject.MultiTenancy
{
    public class Tenant : AbpTenant<Tenant, User>
    {

    }
}