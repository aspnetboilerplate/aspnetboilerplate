using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using Abp.Services;
using System.Collections.Generic;
using Abp.Web.Controllers;
using Castle.DynamicProxy;
using Taskever.Services;
using Taskever.Services.Dto;
using Taskever.Services.Impl;
using Taskever.Web.Dependency;

namespace Taskever.Web.Api
{
    public class AbpServiceApiController<T> : AbpApiController where T : IService
    {
        public AbpServiceApiController()
        {

        }

        //public IList<TaskDto> GetMyTasks()
        //{
        //    return new List<TaskDto>();
        //}
    }

    public class AbpServiceApiControllerInterceptor<TService> : IInterceptor where TService : IService
    {
        private readonly TService _service;

        public AbpServiceApiControllerInterceptor(TService service)
        {
            _service = service;
        }

        public void Intercept(IInvocation invocation)
        {
            if (typeof(TService).IsAssignableFrom(invocation.Method.DeclaringType))
            {
                invocation.ReturnValue = invocation.Method.Invoke(_service, invocation.Arguments);
            }
            else
            {
                invocation.Proceed();
            }
        }
    }

    //  api/services/task/getMyTasks  localhost:51226/api/services/task/getMyTasks

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

    public class Selector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        public Selector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            if (request != null)
            {
                IHttpRouteData routeData = request.GetRouteData();
                if (routeData != null)
                {
                    if (routeData.Route.RouteTemplate == "")
                    {

                    }

                    string serviceName;
                    if (routeData.Values.TryGetValue("serviceName", out serviceName))
                    {
                        if (serviceName == "task")
                        {
                            var desc = new HttpControllerDescriptor(_configuration, "taskServiceController", typeof(AbpServiceApiController<ITaskService>));
                            desc.Properties["servicemethod"] = true;
                            return desc;
                        }
                    }
                }
            }

            return base.SelectController(request);
        }
    }

    public static class DictionaryExtensions
    {
        public static bool TryGetValue<T>(this IDictionary<string, object> collection, string key, out T value)
        {
            object valueObj;
            if (collection.TryGetValue(key, out valueObj))
            {
                if (valueObj is T)
                {
                    value = (T)valueObj;
                    return true;
                }
            }

            value = default(T);
            return false;
        }
    }
}