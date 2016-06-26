using System.Reflection;
using Abp.Application.Services;
using Abp.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Abp.AspNetCore.Mvc.Providers
{
    /// <summary>
    /// Used to add application services as controller.
    /// </summary>
    public class AbpAppServiceControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var type = typeInfo.AsType();

            if (!typeof(IApplicationService).IsAssignableFrom(type) ||
                !type.IsPublic || type.IsAbstract || type.IsGenericType)
            {
                return false;
            }

            //TODO: Also check if given app service is created as controller (using IAbpAspNetCoreConfiguration)

            var remoteServiceAttr = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(type);

            return remoteServiceAttr == null ||
                   remoteServiceAttr.IsEnabledFor(type);
        }
    }
}