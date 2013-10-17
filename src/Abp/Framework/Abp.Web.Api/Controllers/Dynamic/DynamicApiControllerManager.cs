using System.Collections.Generic;
using System.Linq;

namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// This class is used to store dynamic controller informations.
    /// TODO: Use some cache object instead of dictionary?
    /// </summary>
    internal static class DynamicApiControllerManager
    {
        private static readonly IDictionary<string, DynamicApiControllerInfo> DynamicTypes;

        static DynamicApiControllerManager()
        {
            DynamicTypes = new Dictionary<string, DynamicApiControllerInfo>();
        }

        /// <summary>
        /// Searches and returns a dynamic api controller for given name
        /// </summary>
        /// <param name="areaName">Area name</param>
        /// <param name="controllerName">Name of the controller</param>
        /// <returns>Controller info</returns>
        public static DynamicApiControllerInfo FindServiceController(string areaName, string controllerName)
        {
            var registrationName = GetRegistrationName(areaName, controllerName);
            //TODO: Find case insensitive match?
            DynamicApiControllerInfo controllerInfo;
            return DynamicTypes.TryGetValue(registrationName, out controllerInfo) ? controllerInfo : null;
        }

        /// <summary>
        /// Registers given controller info to be found later.
        /// </summary>
        /// <param name="controllerInfo">Controller info</param>
        public static void RegisterServiceController(DynamicApiControllerInfo controllerInfo)
        {
            var registrationName = GetRegistrationName(controllerInfo);

            //TODO: Register case insensitive?
            DynamicTypes[registrationName] = controllerInfo;
        }

        public static List<DynamicApiControllerInfo> GetAllServiceControllers()
        {
            return DynamicTypes.Values.ToList();
        }

        private static string GetRegistrationName(DynamicApiControllerInfo controllerInfo)
        {
            return GetRegistrationName(controllerInfo.AreaName, controllerInfo.Name);
        }

        private static string GetRegistrationName(string areaName, string controllerName)
        {
            return string.IsNullOrWhiteSpace(areaName)
                                       ? controllerName
                                       : areaName + "$" + controllerName;
        }
    }
}