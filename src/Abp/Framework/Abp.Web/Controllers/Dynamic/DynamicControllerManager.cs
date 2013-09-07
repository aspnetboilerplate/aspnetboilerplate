using System.Collections.Concurrent;

namespace Abp.Web.Controllers.Dynamic
{
    /// <summary>
    /// This class is used to store dynamic controller informations.
    /// TODO: Use some cache object instead of dictionary?
    /// </summary>
    internal static class DynamicControllerManager
    {
        private static readonly ConcurrentDictionary<string, DynamicControllerInfo> DynamicTypes = new ConcurrentDictionary<string, DynamicControllerInfo>();

        /// <summary>
        /// Searches and returns a dynamic api controller for given name
        /// </summary>
        /// <param name="controllerName">Name of the controller</param>
        /// <returns>Controller info</returns>
        public static DynamicControllerInfo FindServiceController(string controllerName)
        {
            //TODO: Find case insensitive match!
            DynamicControllerInfo controllerInfo;
            return DynamicTypes.TryGetValue(controllerName, out controllerInfo) ? controllerInfo : null;
        }

        /// <summary>
        /// Registers given controller info to be found later.
        /// </summary>
        /// <param name="controllerInfo">Controller info</param>
        public static void RegisterServiceController(DynamicControllerInfo controllerInfo)
        {
            //TODO: Register case insensitive!
            DynamicTypes[controllerInfo.Name] = controllerInfo;
        }
    }
}