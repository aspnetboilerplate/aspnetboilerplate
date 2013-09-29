using System.Collections.Generic;

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
        /// <param name="controllerName">Name of the controller</param>
        /// <returns>Controller info</returns>
        public static DynamicApiControllerInfo FindServiceController(string controllerName)
        {
            //TODO: Find case insensitive match!
            DynamicApiControllerInfo controllerInfo;
            return DynamicTypes.TryGetValue(controllerName, out controllerInfo) ? controllerInfo : null;
        }

        /// <summary>
        /// Registers given controller info to be found later.
        /// </summary>
        /// <param name="controllerInfo">Controller info</param>
        public static void RegisterServiceController(DynamicApiControllerInfo controllerInfo)
        {
            //TODO: Register case insensitive?
            DynamicTypes[controllerInfo.Name] = controllerInfo;
        }
    }
}