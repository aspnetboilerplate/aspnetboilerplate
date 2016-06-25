using Abp.Application.Services;
using Abp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Abp.AspNetCore.Mvc.Conventions
{
    public class AbpAppServiceConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (typeof(IApplicationService).IsAssignableFrom(controller.ControllerType))
                {
                    NormalizeControllerName(controller);

                    controller.ApiExplorer.IsVisible = true;
                    controller.ApiExplorer.GroupName = controller.ControllerName;
                    //TODO: Do it for action level for better control!
                    var selectorModel = new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(
                            new RouteAttribute("api/services/app/" + controller.ControllerName + "/[action]"
                            )
                        )
                    };

                    controller.Selectors.Clear();
                    controller.Selectors.Add(selectorModel);
                }
            }
        }

        private static void NormalizeControllerName(ControllerModel controller)
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