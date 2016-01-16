using System.Reflection;

namespace Abp.WebApi.Swagger.Builders
{
    public static class AbpSwaggerBuilder
    {
        private static readonly AbpSwaggerModel AbpSwaggerModel;

        static AbpSwaggerBuilder()
        {
            AbpSwaggerModel = new AbpSwaggerModel();
        }

        public static IBatchAbpSwaggerBuilder<T> ForAll<T>(Assembly assembly, string servicePrefix = "app")
        {
            return new BatchAbpSwaggerBuilder<T>(AbpSwaggerModel, assembly, servicePrefix);
        }
    }
}
