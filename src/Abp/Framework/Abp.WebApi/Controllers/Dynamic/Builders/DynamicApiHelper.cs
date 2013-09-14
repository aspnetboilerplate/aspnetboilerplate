namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// A helper class for dynamic api controllers.
    /// </summary>
    internal class DynamicApiHelper
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
    }
}
