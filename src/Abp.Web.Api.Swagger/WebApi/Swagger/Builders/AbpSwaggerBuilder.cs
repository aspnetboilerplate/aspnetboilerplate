using System.Reflection;

namespace Abp.WebApi.Swagger.Builders
{
    /// <summary>
    /// Used to generate swagger document
    /// </summary>
    public static class AbpSwaggerBuilder
    {
        /// <summary>
        /// Generate swagger document for multiple abp application service.
        /// </summary>
        /// <typeparam name="T">Base type (class or interface) for services</typeparam>
        /// <param name="assembly">Assembly contains types</param>
        /// <param name="servicePrefix">Service prefix</param>
        /// <returns></returns>
        public static IBatchAbpSwaggerBuilder<T> ForAll<T>(Assembly assembly, string servicePrefix = "app")
        {
            return new BatchAbpSwaggerBuilder<T>(assembly, servicePrefix);
        }
    }
}
