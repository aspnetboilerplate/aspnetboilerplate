using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Dependency;

namespace Abp.WebApi.Controllers
{
    /// <summary>
    /// This class is used to use IOC system to create api controllers.
    /// It's used by ASP.NET system.
    /// </summary>
    public class AbpControllerActivator : IHttpControllerActivator
    {
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controllerWrapper = IocManager.Instance.ResolveAsDisposable<IHttpController>(controllerType);
            request.RegisterForDispose(controllerWrapper);
            return controllerWrapper.Object;
        }
    }
}