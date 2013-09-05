using System.Web.Http.Controllers;

namespace Abp.Web.Controllers.Dynamic
{
    public class AbpControllerActionSelector : ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            object serviceName;
            if ((bool)controllerContext.ControllerDescriptor.Properties.TryGetValue("servicemethod", out  serviceName))
            {
                return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, controllerContext.Controller.GetType().GetMethod("GetMyTasks"));
            }

            return base.SelectAction(controllerContext);
        }
    }
}