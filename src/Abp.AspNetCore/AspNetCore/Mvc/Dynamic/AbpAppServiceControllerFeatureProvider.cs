using System.Reflection;
using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Abp.AspNetCore.Mvc.Dynamic
{
    public class AbpAppServiceControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            return IsAppService(typeInfo);
        }

        protected virtual bool IsAppService(TypeInfo typeInfo)
        {
            return typeof(IApplicationService).IsAssignableFrom(typeInfo.GetTypeInfo()) &&
                   !typeInfo.IsAbstract &&
                   !typeInfo.IsGenericType &&
                   typeInfo.IsPublic;
        }
    }
}