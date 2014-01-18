using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Utils.Extensions;
using Abp.Utils.Extensions.Collections;

namespace Abp.WebApi.Controllers.Dynamic.Selectors
{
    /// <summary>
    /// This class is used to extend default controller selector to add dynamic api controller creation feature of Abp.
    /// It checks if requested controller is a dynamic api controller, if it is,
    /// returns <see cref="HttpControllerDescriptor"/> to ASP.NET system.
    /// </summary>
    public class AbpHttpControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        /// <summary>
        /// Creates a new <see cref="AbpHttpControllerSelector"/> object.
        /// </summary>
        /// <param name="configuration">Http configuration</param>
        public AbpHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method is called by Web API system to select the controller for this request.
        /// </summary>
        /// <param name="request">Request object</param>
        /// <returns>The controller to be used</returns>
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            if (request != null)
            {
                var routeData = request.GetRouteData();
                if (routeData != null)
                {
                    string serviceName;
                    if (routeData.Values.TryGetValue("serviceName", out serviceName))
                    {
                        string areaName;
                        if (routeData.Values.TryGetValue("areaName", out areaName))
                        {
                            var controllerName = areaName.ToPascalCase() + "/" + serviceName.ToPascalCase();

                            var controllerInfo = DynamicApiControllerManager.Find(controllerName);
                            if (controllerInfo != null)
                            {
                                var controllerDescriptor = new HttpControllerDescriptor(_configuration, controllerInfo.Name, controllerInfo.Type);
                                controllerDescriptor.Properties["__AbpDynamicApiControllerInfo"] = controllerInfo;
                                return controllerDescriptor;
                            }
                        }
                    }
                }
            }

            return base.SelectController(request);
        }
    }
}