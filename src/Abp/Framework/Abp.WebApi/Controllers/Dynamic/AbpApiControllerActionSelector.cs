using System.Web.Http.Controllers;

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
            //TODO: Implement real logic!!!
            object serviceName;
            if (controllerContext.ControllerDescriptor.Properties.TryGetValue("servicemethod", out  serviceName))
            {
                return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, controllerContext.Controller.GetType().GetMethod("GetMyTasks"));
            }

            return base.SelectAction(controllerContext);
        }
    }
}