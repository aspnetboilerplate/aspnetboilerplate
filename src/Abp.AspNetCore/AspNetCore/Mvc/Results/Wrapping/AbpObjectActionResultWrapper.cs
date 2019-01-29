using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Linq;
using Abp.Dependency;
using Abp.Json;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpObjectActionResultWrapper : IAbpActionResultWrapper
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly ConcurrentDictionary<Type, AbpMvcContractResolver> SharedContractResolver =
            new ConcurrentDictionary<Type, AbpMvcContractResolver>();

        public AbpObjectActionResultWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Wrap(ResultExecutingContext actionResult, WrapResultAttribute wrapResultAttribute)
        {
            var objectResult = actionResult.Result as ObjectResult;
            if (objectResult == null)
            {
                throw new ArgumentException($"{nameof(actionResult)} should be ObjectResult!");
            }

            if (objectResult.Value is AjaxResponseBase)
            {
                return;
            }

            objectResult.Value = new AjaxResponse(objectResult.Value);
            if (objectResult.Formatters.Any(f => f is JsonOutputFormatter))
            {
                return;
            }

            var serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();
            serializerSettings.ContractResolver = SharedContractResolver.GetOrAdd(
                wrapResultAttribute.NamingStrategyType, namingStrategyType =>
                    new AbpMvcContractResolver(_serviceProvider.GetRequiredService<IIocResolver>())
                    {
                        NamingStrategy =
                            (NamingStrategy)Activator.CreateInstance(namingStrategyType)
                    });

            objectResult.Formatters.Add(
                new JsonOutputFormatter(
                    serializerSettings,
                    _serviceProvider.GetRequiredService<ArrayPool<char>>()
                )
            );
        }
    }
}