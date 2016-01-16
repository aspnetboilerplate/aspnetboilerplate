namespace Abp.WebApi.Swagger.Configuration
{
    public interface IAbpSwaggerEnabledConfiguration
    {
        void EnableAbpSwaggerUi();

        void EnableAbpSwaggerUi(string routeTemplate);
    }
}
