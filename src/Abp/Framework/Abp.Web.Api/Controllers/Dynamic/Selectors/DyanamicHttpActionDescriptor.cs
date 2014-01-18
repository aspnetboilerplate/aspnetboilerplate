using System;
using System.Reflection;
using System.Web.Http.Controllers;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers.Dynamic.Selectors
{
    public class DyanamicHttpActionDescriptor : ReflectedHttpActionDescriptor
    {
        public override Type ReturnType
        {
            get
            {
                return typeof(AbpAjaxResponse);
            }
        }

        public DyanamicHttpActionDescriptor(HttpControllerDescriptor controllerDescriptor, MethodInfo methodInfo)
            :base(controllerDescriptor, methodInfo)
        {
            
        }

        public override System.Threading.Tasks.Task<object> ExecuteAsync(HttpControllerContext controllerContext, System.Collections.Generic.IDictionary<string, object> arguments, System.Threading.CancellationToken cancellationToken)
        {
            return base
                .ExecuteAsync(controllerContext, arguments, cancellationToken)
                .ContinueWith(
                    task => (object) new AbpAjaxResponse(task.Result));
        }
    }
}