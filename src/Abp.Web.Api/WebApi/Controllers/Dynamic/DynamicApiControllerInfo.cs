using System;
using System.Collections.Generic;
using System.Web.Http.Filters;

namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// Used to store a controller information.
    /// </summary>
    public class DynamicApiControllerInfo
    {
        /// <summary>
        /// Name of the service.
        /// </summary>
        public string ServiceName { get; private set; }

        /// <summary>
        /// Service interface type.
        /// </summary>
        public Type ServiceInterfaceType { get; private set; }

        /// <summary>
        /// Api Controller type.
        /// </summary>
        public Type ApiControllerType { get; private set; }

        /// <summary>
        /// Interceptor type.
        /// </summary>
        public Type InterceptorType { get; private set; }

        /// <summary>
        /// Is API Explorer enabled.
        /// </summary>
        public bool? IsApiExplorerEnabled { get; private set; }

        /// <summary>
        /// Dynamic Action Filters for this controller.
        /// </summary>
        public IFilter[] Filters { get; set; }

        /// <summary>
        /// All actions of the controller.
        /// </summary>
        public IDictionary<string, DynamicApiActionInfo> Actions { get; private set; }

        /// <summary>
        /// Creates a new <see cref="DynamicApiControllerInfo"/> instance.
        /// </summary>
        /// <param name="serviceName">Name of the service</param>
        /// <param name="serviceInterfaceType">Service interface type</param>
        /// <param name="apiControllerType">Api Controller type</param>
        /// <param name="interceptorType">Interceptor type</param>
        /// <param name="filters">Filters</param>
        /// <param name="isApiExplorerEnabled">Is API explorer enabled</param>
        public DynamicApiControllerInfo(
            string serviceName, 
            Type serviceInterfaceType, 
            Type apiControllerType, 
            Type interceptorType, 
            IFilter[] filters = null,
            bool? isApiExplorerEnabled = null)
        {
            ServiceName = serviceName;
            ServiceInterfaceType = serviceInterfaceType;
            ApiControllerType = apiControllerType;
            InterceptorType = interceptorType;
            IsApiExplorerEnabled = isApiExplorerEnabled;
            Filters = filters ?? new IFilter[] { }; //Assigning or initialzing the action filters.

            Actions = new Dictionary<string, DynamicApiActionInfo>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}