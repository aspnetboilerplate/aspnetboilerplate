using System;
using System.Collections.Generic;
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
        /// Controller type.
        /// </summary>
        public Type Type { get; private set; }

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
        /// <param name="type">Controller type</param>
        /// <param name="filters">Filters</param>
        public DynamicApiControllerInfo(string serviceName, Type type, IFilter[] filters = null)
        {
            ServiceName = serviceName;
            Type = type;
            Filters = filters ?? new IFilter[] { }; //Assigning or initialzing the action filters.

            Actions = new Dictionary<string, DynamicApiActionInfo>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}