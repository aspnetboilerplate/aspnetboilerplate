using System;
using Abp.WebApi.Swagger.Configuration;

namespace Abp.WebApi.Swagger.Builders
{
    public interface IBatchAbpSwaggerBuilder<T>
    {
        IBatchAbpSwaggerBuilder<T> RelativePath(string relativePath);

        IBatchAbpSwaggerBuilder<T> Where(Func<Type, bool> predicate);

        IBatchAbpSwaggerBuilder<T> WithServiceName(Func<Type, string> serviceNameSelector);

        IAbpSwaggerEnabledConfiguration Build(string moduleName);
    }
}
