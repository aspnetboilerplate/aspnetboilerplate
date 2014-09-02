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
        /// Dyanmic proxied type.
        /// </summary>
        public Type ProxiedType { get; private set; }

        /// <summary>
        /// All actions of the controller.
        /// </summary>
        public IDictionary<string, DynamicApiActionInfo> Actions { get; private set; }

        /// <summary>
        /// Dynamic ActionFilters for this controller.
        /// </summary>
        public IList<IFilter> ActionFilters { get; set; }

        /// <summary>
        /// Creates a new <see cref="DynamicApiControllerInfo"/> instance.
        /// </summary>
        /// <param name="serviceName">Name of the service</param>
        /// <param name="type">Controller type</param>
        /// <param name="proxiedType">Proxied type</param>
        public DynamicApiControllerInfo(string serviceName, Type type, Type proxiedType)
        {
            ServiceName = serviceName;
            Type = type;
            ProxiedType = proxiedType;

            Actions = new Dictionary<string, DynamicApiActionInfo>(StringComparer.InvariantCultureIgnoreCase); //TODO@Halil: Test ignoring case

            ActionFilters = new List<IFilter>(); //Intializing ActionFilters
        }
    }
}