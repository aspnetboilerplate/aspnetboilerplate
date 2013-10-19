using System.Globalization;
using System.Web.Http.Controllers;
using Abp.Exceptions;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic
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
            //TODO: If method is not supplied, try to guess the method by Http Verb and parameters ?

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
                var actionName = (controllerContext.RouteData.Values["action"] as string);
                if (string.IsNullOrWhiteSpace(actionName))
                {
                    throw new AbpException("There is no action specified.");
                }

                //Get action information
                actionName = (controllerContext.RouteData.Values["action"] as string).ToPascalCase();
                if (!controllerInfo.Actions.ContainsKey(actionName))
                {
                    throw new AbpException("There is no action " + actionName + " defined for api controller " + controllerInfo.Name);
                }
                
                return new DyanamicHttpActionDescriptor(controllerContext.ControllerDescriptor, controllerInfo.Actions[actionName].Method);
            }
            
            return base.SelectAction(controllerContext);
        }
    }
}