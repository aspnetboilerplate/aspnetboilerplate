using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Abp.Extensions;

namespace Abp.Web.Mvc.Extensions
{
    public static class ActionDescriptorExtensions
    {
        public static MethodInfo GetMethodInfoOrNull(this ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ReflectedActionDescriptor)
            {
                return actionDescriptor.As<ReflectedActionDescriptor>().MethodInfo;
            }

            if (actionDescriptor is ReflectedAsyncActionDescriptor)
            {
                return actionDescriptor.As<ReflectedAsyncActionDescriptor>().MethodInfo;
            }

            if (actionDescriptor is TaskAsyncActionDescriptor)
            {
                return actionDescriptor.As<TaskAsyncActionDescriptor>().MethodInfo;
            }

            return null;
        }
    }
}