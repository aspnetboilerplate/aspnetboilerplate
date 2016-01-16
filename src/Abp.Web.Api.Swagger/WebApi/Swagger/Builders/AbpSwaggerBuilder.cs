using System.Reflection;

namespace Abp.WebApi.Swagger.Builders
{
    public static class AbpSwaggerBuilder
    {
        public static IBatchAbpSwaggerBuilder<T> ForAll<T>(Assembly assembly, string servicePrefix = "app")
        {
            return new BatchAbpSwaggerBuilder<T>(assembly, servicePrefix);
        }
    }
}
