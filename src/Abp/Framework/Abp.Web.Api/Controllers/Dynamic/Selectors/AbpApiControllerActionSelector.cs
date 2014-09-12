using System.Web.Http.Controllers;
using Abp.WebApi.Controllers.Dynamic.Builders;

namespace Abp.WebApi.Controllers.Dynamic.Selectors
{
    /// <summary>
    /// This class overrides ApiControllerActionSelector to select actions of dynamic ApiControllers.
    /// </summary>
    public class AbpApiControllerActionSelector : ApiControllerActionSelector
    {
        /// <summary>
        /// This class is called by Web API system to select action method from given controller.
        /// </summary>
        /// <param name="controllerContext">Controller context</param>
        /// <returns>Action to be used</returns>
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            object controllerInfoObj;
            if (controllerContext.ControllerDescriptor.Properties.TryGetValue("__AbpDynamicApiControllerInfo", out  controllerInfoObj))
            {
                //Get controller information which is selected by AbpHttpControllerSelector.
                var controllerInfo = controllerInfoObj as DynamicApiControllerInfo;
                if (controllerInfo == null)
                {
                    throw new AbpException("__AbpDynamicApiControllerInfo in ControllerDescriptor.Properties is not a " + typeof(DynamicApiControllerInfo).FullName + " class.");
                }

                //Get action name
                var serviceNameWithAction = (controllerContext.RouteData.Values["serviceNameWithAction"] as string);
                if (serviceNameWithAction != null)
                {
                    var actionName = DynamicApiServiceNameHelper.GetActionNameInServiceNameWithAction(serviceNameWithAction);

                    //Get action information
                    if (!controllerInfo.Actions.ContainsKey(actionName))
                    {
                        throw new AbpException("There is no action " + actionName + " defined for api controller " + controllerInfo.ServiceName);
                    }

                    return new DyanamicHttpActionDescriptor(controllerContext.ControllerDescriptor, controllerInfo.Actions[actionName].Method, controllerInfo.Actions[actionName].Filters);
                }
            }
            
            return base.SelectAction(controllerContext);
        }
    }
}