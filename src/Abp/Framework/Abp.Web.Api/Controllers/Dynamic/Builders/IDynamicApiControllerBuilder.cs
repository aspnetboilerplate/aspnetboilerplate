namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// This interface is used to define a dynamic api controller.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    public interface IApiControllerBuilder<T>
    {
        /// <summary>
        /// Used to specify a method definition.
        /// </summary>
        /// <param name="methodName">Name of the method in proxied type</param>
        /// <returns>Action builder</returns>
        IApiControllerActionBuilder<T> ForMethod(string methodName);
        
        /// <summary>
        /// Sets name of the api controller.
        /// </summary>
        /// <param name="controllerName">Api controller name</param>
        /// <returns>Controller builder</returns>
        IApiControllerBuilder<T> WithControllerName(string controllerName);

        /// <summary>
        /// Sets area name of the api controller.
        /// </summary>
        /// <param name="areaName">area name</param>
        /// <returns>Controller builder</returns>
        IApiControllerBuilder<T> WithAreaName(string areaName);
        
        /// <summary>
        /// Builds the controller.
        /// This method must be called at last of the build operation.
        /// </summary>
        void Build();

        /// <summary>
        /// Used to tell builder to use conventions for api.
        /// </summary>
        /// <returns>Controller builder</returns>
        IApiControllerBuilder<T> UseConventions();
    }
}