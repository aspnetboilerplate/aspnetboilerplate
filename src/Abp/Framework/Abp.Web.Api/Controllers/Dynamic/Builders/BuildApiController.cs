namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// Used to generate dynamic api controllers for arbitrary types.
    /// </summary>
    public static class BuildApiController
    {
        /// <summary>
        /// Generates a new dynamic api controller for given type.
        /// </summary>
        /// <typeparam name="T">Type of the proxied object</typeparam>
        public static IApiControllerBuilder<T> For<T>(string controllerName)
        {
            return new ApiControllerBuilder<T>(controllerName);
        }
    }
}
