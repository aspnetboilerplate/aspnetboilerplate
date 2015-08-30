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
            if (!controllerContext.ControllerDescriptor.Properties.TryGetValue("__AbpDynamicApiControllerInfo", out controllerInfoObj))
            {
                return base.SelectAction(controllerContext);
            }

            //Get controller information which is selected by AbpHttpControllerSelector.
            var controllerInfo = controllerInfoObj as DynamicApiControllerInfo;
            if (controllerInfo == null)
            {
                throw new AbpException("__AbpDynamicApiControllerInfo in ControllerDescriptor.Properties is not a " + typeof(DynamicApiControllerInfo).FullName + " class.");
            }

            //Get action name
            var serviceNameWithAction = (controllerContext.RouteData.Values["serviceNameWithAction"] as string);
            if (serviceNameWithAction == null)
            {
                return base.SelectAction(controllerContext);
            }

            var actionName = DynamicApiServiceNameHelper.GetActionNameInServiceNameWithAction(serviceNameWithAction);

            //Get action information
            DynamicApiActionInfo actionInfo;
            if (!controllerInfo.Actions.TryGetValue(actionName, out actionInfo))
            {
                throw new AbpException("There is no action " + actionName + " defined for api controller " + controllerInfo.ServiceName);
            }

            if (!actionInfo.Verb.IsEqualTo(controllerContext.Request.Method))
            {
                throw new AbpException(
                    "There is an action " + actionName +
                    " defined for api controller " + controllerInfo.ServiceName + 
                    " but with a different HTTP Verb. Request verb is " + controllerContext.Request.Method + 
                    ". It should be " + actionInfo.Verb);                
            }

            return new DyanamicHttpActionDescriptor(controllerContext.ControllerDescriptor, actionInfo.Method, actionInfo.Filters);
        }
    }
}