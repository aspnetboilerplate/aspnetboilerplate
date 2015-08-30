using System;
using System.Web.Http.Filters;
using Abp.Web;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// This interface is used to define a dynamic api controllers.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    public interface IBatchApiControllerBuilder<T>
    {
        /// <summary>
        /// Used to filter types.
        /// </summary>
        /// <param name="predicate">Predicate to filter types</param>
        IBatchApiControllerBuilder<T> Where(Func<Type, bool> predicate);

        /// <summary>
        /// The adds Action filters for the Dynamic Controller.
        /// </summary>
        /// <param name="filters"> The filters. </param>
        /// <returns>The current Controller Builder </returns>
        IBatchApiControllerBuilder<T> WithFilters(params IFilter[] filters);

        /// <summary>
        /// Selects service name for a controller.
        /// </summary>
        /// <param name="serviceNameSelector">Service name selector</param>
        /// <returns></returns>
        IBatchApiControllerBuilder<T> WithServiceName(Func<Type, string> serviceNameSelector);
        
        /// <summary>
        /// Use conventional Http Verbs by method names.
        /// By default, it uses <see cref="HttpVerb.Post"/> for all actions.
        /// </summary>
        /// <returns>The current Controller Builder</returns>
        IBatchApiControllerBuilder<T> WithConventionalVerbs();

        /// <summary>
        /// Builds the controller.
        /// This method must be called at last of the build operation.
        /// </summary>
        void Build();
    }
}