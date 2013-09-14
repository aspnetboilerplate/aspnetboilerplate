namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// A helper class for dynamic api controllers.
    /// </summary>
    internal static class DynamicApiHelper
    {
        /// <summary>
        /// Gets conventional controller name for given type.
        /// </summary>
        /// <typeparam name="T">Type to get controller name</typeparam>
        /// <returns>Controller name</returns>
        public static string GetConventionalControllerName<T>()
        {
            var type = typeof(T);
            var name = type.Name;

            //Skip I letter for interface names
            if (name.Length > 1 && type.IsInterface)
            {
                name = name.Substring(1);
            }

            //Remove "Service" from end as convention
            if (name.EndsWith("Service") && name.Length > 7)
            {
                name = name.Substring(0, name.Length - 7);
            }

            return name;
        }

        public static HttpVerb GetConventionalVerbForMethodName(string methodName)
        {
            if (methodName.StartsWith("Get"))
            {
                return HttpVerb.Get;
            }

            if (methodName.StartsWith("Update") || methodName.StartsWith("Put"))
            {
                return HttpVerb.Put;
            }

            if (methodName.StartsWith("Delete") || methodName.StartsWith("Remove"))
            {
                return HttpVerb.Delete;
            }

            return GetDefaultHttpVerb();
        }

        public static HttpVerb GetDefaultHttpVerb()
        {
            return HttpVerb.Get;
        }
    }
}
