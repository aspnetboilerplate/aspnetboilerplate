using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Abp.Web.Mvc.Controllers
{
    internal static class ActionDescriptorHelper
    {
        public static MethodInfo GetMethodInfo(ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ReflectedActionDescriptor)
            {
                return ((ReflectedActionDescriptor)actionDescriptor).MethodInfo;
            }

            if (actionDescriptor is ReflectedAsyncActionDescriptor)
            {
                return ((ReflectedAsyncActionDescriptor)actionDescriptor).MethodInfo;
            }

            if (actionDescriptor is TaskAsyncActionDescriptor)
            {
                return ((TaskAsyncActionDescriptor)actionDescriptor).MethodInfo;
            }

            throw new AbpException("Could not get MethodInfo for the action '" + actionDescriptor.ActionName + "' of controller '" + actionDescriptor.ControllerDescriptor.ControllerName + "'.");
        }
    }
}
