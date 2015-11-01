using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Web.Http.Filters;

namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// Used to store a controller information.
    /// </summary>
    internal class DynamicApiControllerInfo
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
        public DynamicApiControllerInfo(string serviceName, Type serviceInterfaceType, Type apiControllerType, Type interceptorType, IFilter[] filters = null)
        {
            ServiceName = serviceName;
            ServiceInterfaceType = serviceInterfaceType;
            ApiControllerType = apiControllerType;
            InterceptorType = interceptorType;
            Filters = filters ?? new IFilter[] { }; //Assigning or initialzing the action filters.

            Actions = new Dictionary<string, DynamicApiActionInfo>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}