using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Collections.Extensions;

namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// This class is used to store dynamic controller information.
    /// </summary>
    internal static class DynamicApiControllerManager
    {
        private static readonly IDictionary<string, DynamicApiControllerInfo> DynamicApiControllers;

        static DynamicApiControllerManager()
        {
            DynamicApiControllers = new Dictionary<string, DynamicApiControllerInfo>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Registers given controller info to be found later.
        /// </summary>
        /// <param name="controllerInfo">Controller info</param>
        public static void Register(DynamicApiControllerInfo controllerInfo)
        {
            DynamicApiControllers[controllerInfo.ServiceName] = controllerInfo;
        }

        /// <summary>
        /// Searches and returns a dynamic api controller for given name.
        /// </summary>
        /// <param name="controllerName">Name of the controller</param>
        /// <returns>Controller info</returns>
        public static DynamicApiControllerInfo FindOrNull(string controllerName)
        {
            return DynamicApiControllers.GetOrDefault(controllerName);
        }

        public static IReadOnlyList<DynamicApiControllerInfo> GetAll()
        {
            return DynamicApiControllers.Values.ToImmutableList();
        }
    }
}