using Abp.Application.Services;
using Abp.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Abp.AspNetCore.Mvc.Providers
{
    public class AbpApplicationModelProvider : IApplicationModelProvider
    {
        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            foreach (var controller in context.Result.Controllers)
            {
                if (typeof(IApplicationService).IsAssignableFrom(controller.ControllerType))
                {
                    if (controller.ControllerName.EndsWith("AppService"))
                    {
                        controller.ControllerName = controller.ControllerName.Left(controller.ControllerName.Length - "AppService".Length);
                    }
                    else if (controller.ControllerName.EndsWith("Service"))
                    {
                        controller.ControllerName = controller.ControllerName.Left(controller.ControllerName.Length - "Service".Length);
                    }
                }
            }
        }

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {

        }

        public int Order => (-1000 + 20);
    }
}