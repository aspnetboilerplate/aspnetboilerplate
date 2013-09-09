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
            //TODO: If method is not supplied, try to guess the method by Http Verb and parameters!

            object controllerInfoObj;
            if (controllerContext.ControllerDescriptor.Properties.TryGetValue("AbpDynamicApiControllerInfo", out  controllerInfoObj))
            {
                var controllerInfo = (DynamicApiControllerInfo)controllerInfoObj;
                var methodName = ((string)controllerContext.RouteData.Values["methodName"]).ToPascalCase();

                if (!controllerInfo.Methods.ContainsKey(methodName))
                {
                    throw new AbpException("There is no action " + methodName + " defined for api controller " + controllerInfo.Name);
                }

                return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, controllerInfo.Methods[methodName].Method);
            }

            return base.SelectAction(controllerContext);
        }
    }
}