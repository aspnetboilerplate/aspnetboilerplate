using System;
using Abp.AspNetCore.Configuration;
using Abp.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Abp.AspNetCore.Dependency;

public static class AbpMvcBuilderExtensions
{
    public static IMvcBuilder AddAbpNewtonsoftJson(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddNewtonsoftJson().Services.AddOptions<MvcNewtonsoftJsonOptions>()
            .Configure<IServiceProvider>((options, rootServiceProvider) =>
            {
                var aspNetCoreConfiguration = rootServiceProvider.GetRequiredService<IAbpAspNetCoreConfiguration>();
                options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(aspNetCoreConfiguration.InputDateTimeFormats, aspNetCoreConfiguration.OutputDateTimeFormat)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                options.SerializerSettings.Converters.Add(new CultureInvariantDecimalConverter());
                options.SerializerSettings.Converters.Add(new CultureInvariantDoubleConverter());
            });

        return mvcBuilder;
    }
}
