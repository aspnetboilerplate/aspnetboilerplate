using System;
using Abp.WebApi.Swagger.Configuration;

namespace Abp.WebApi.Swagger.Builders
{
    /// <summary>
    /// Use this interface to chain call to configure and build swagger document.
    /// </summary>
    /// <typeparam name="T">Base type (class or interface) for services</typeparam>
    public interface IBatchAbpSwaggerBuilder<T>
    {
        /// <summary>
        /// Set relative path you want save swagger document
        /// </summary>
        /// <param name="relativePath">Relative path</param>
        /// <returns></returns>
        IBatchAbpSwaggerBuilder<T> RelativePath(string relativePath);

        /// <summary>
        /// Used to filter types.
        /// </summary>
        /// <param name="predicate">Predicate to filter types</param>
        IBatchAbpSwaggerBuilder<T> Where(Func<Type, bool> predicate);

        /// <summary>
        /// Selects service name for a controller.
        /// </summary>
        /// <param name="serviceNameSelector">Service name selector</param>
        /// <returns></returns>
        IBatchAbpSwaggerBuilder<T> WithServiceName(Func<Type, string> serviceNameSelector);

        /// <summary>
        /// Build swagger document
        /// </summary>
        /// <param name="moduleName">module name if you have multi-module</param>
        /// <returns></returns>
        IAbpSwaggerEnabledConfiguration Build(string moduleName);
    }
}
