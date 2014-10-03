using System.Reflection;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// Used to generate dynamic api controllers for arbitrary types.
    /// </summary>
    public static class DynamicApiControllerBuilder
    {
        /// <summary>
        /// Generates a new dynamic api controller for given type.
        /// </summary>
        /// <param name="serviceName">Name of the Api controller service. For example: 'myapplication/myservice'.</param>
        /// <typeparam name="T">Type of the proxied object</typeparam>
        public static IApiControllerBuilder<T> For<T>(string serviceName)
        {
            return new ApiControllerBuilder<T>(serviceName);
        }

        /// <summary>
        /// Generates multiple dynamic api controllers.
        /// </summary>
        /// <typeparam name="T">Base type (class or interface) for services</typeparam>
        /// <param name="assembly">Assembly contains types</param>
        /// <param name="servicePrefix">Service prefix</param>
        public static IBatchApiControllerBuilder<T> ForAll<T>(Assembly assembly, string servicePrefix)
        {
            return new BatchApiControllerBuilder<T>(assembly, servicePrefix);
        }
    }
}
