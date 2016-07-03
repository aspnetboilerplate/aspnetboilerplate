using Abp.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Abp.AspNetCore.Mvc.Authorization
{
    public class AbpMvcAllowAnonymousAttribute : AllowAnonymousAttribute, IAbpAllowAnonymousAttribute
    {
    
    }
}