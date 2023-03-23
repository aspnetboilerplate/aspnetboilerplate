using System;
using Abp.AspNetCore.Configuration;
using Abp.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

                if (aspNetCoreConfiguration.UseMvcDateTimeFormatForAppServices)
                {
                    var mvcNewtonsoftJsonOptions = rootServiceProvider.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value;
                    aspNetCoreConfiguration.InputDateTimeFormats.Add(mvcNewtonsoftJsonOptions.SerializerSettings.DateFormatString);
                }

                options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(aspNetCoreConfiguration.InputDateTimeFormats, aspNetCoreConfiguration.OutputDateTimeFormat)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

        return mvcBuilder;
    }
}
