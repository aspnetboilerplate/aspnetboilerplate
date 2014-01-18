using System;
using System.Collections.Generic;

namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// This class is used to store dynamic controller informations.
    /// </summary>
    internal static class DynamicApiControllerManager
    {
        private static readonly IDictionary<string, DynamicApiControllerInfo> DynamicApiControllers;

        static DynamicApiControllerManager()
        {
            DynamicApiControllers = new Dictionary<string, DynamicApiControllerInfo>(StringComparer.InvariantCultureIgnoreCase); //TODO: Test it
        }

        /// <summary>
        /// Registers given controller info to be found later.
        /// </summary>
        /// <param name="controllerInfo">Controller info</param>
        public static void Register(DynamicApiControllerInfo controllerInfo)
        {
            DynamicApiControllers[controllerInfo.Name] = controllerInfo;
        }

        /// <summary>
        /// Searches and returns a dynamic api controller for given name.
        /// </summary>
        /// <param name="controllerName">Name of the controller</param>
        /// <returns>Controller info</returns>
        public static DynamicApiControllerInfo Find(string controllerName)
        {
            DynamicApiControllerInfo controllerInfo;
            return DynamicApiControllers.TryGetValue(controllerName, out controllerInfo) ? controllerInfo : null;
        }
    }
}