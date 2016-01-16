namespace Abp.WebApi.Swagger.Configuration
{
    /// <summary>
    /// Use to enable swagger ui
    /// </summary>
    public interface IAbpSwaggerEnabledConfiguration
    {
        /// <summary>
        /// Enable abp swagger ui
        /// </summary>
        void EnableAbpSwaggerUi();

        /// <summary>
        /// Enable abp swagger ui
        /// </summary>
        /// <param name="routeTemplate">Route template</param>
        void EnableAbpSwaggerUi(string routeTemplate);
    }
}
