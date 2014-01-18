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
            var registrationName = GetRegistrationName(controllerInfo);
            DynamicApiControllers[registrationName] = controllerInfo;
        }

        /// <summary>
        /// Searches and returns a dynamic api controller for given name.
        /// </summary>
        /// <param name="areaName">Area name</param>
        /// <param name="controllerName">Name of the controller</param>
        /// <returns>Controller info</returns>
        public static DynamicApiControllerInfo Find(string areaName, string controllerName)
        {
            var registrationName = GetRegistrationName(areaName, controllerName);
            DynamicApiControllerInfo controllerInfo;
            return DynamicApiControllers.TryGetValue(registrationName, out controllerInfo) ? controllerInfo : null;
        }

        #region Private methods

        private static string GetRegistrationName(DynamicApiControllerInfo controllerInfo)
        {
            return GetRegistrationName(controllerInfo.AreaName, controllerInfo.Name);
        }

        private static string GetRegistrationName(string areaName, string controllerName)
        {
            return !string.IsNullOrWhiteSpace(areaName) ? (areaName + "/" + controllerName) : controllerName;
        }

        #endregion
    }
}