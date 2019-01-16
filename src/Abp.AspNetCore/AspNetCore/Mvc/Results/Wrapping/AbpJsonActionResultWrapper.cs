using System;
using System.Collections.Concurrent;
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
    public class AbpJsonActionResultWrapper : IAbpActionResultWrapper
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly ConcurrentDictionary<Type, AbpMvcContractResolver> SharedContractResolver =
            new ConcurrentDictionary<Type, AbpMvcContractResolver>();

        public AbpJsonActionResultWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Wrap(ResultExecutingContext actionResult, WrapResultAttribute wrapResultAttribute)
        {
            var jsonResult = actionResult.Result as JsonResult;
            if (jsonResult == null)
            {
                throw new ArgumentException($"{nameof(actionResult)} should be JsonResult!");
            }

            if (!(jsonResult.Value is AjaxResponseBase))
            {
                jsonResult.Value = new AjaxResponse(jsonResult.Value);
            }

            if (jsonResult.SerializerSettings == null)
            {
                jsonResult.SerializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();
            }

            jsonResult.SerializerSettings.ContractResolver = SharedContractResolver.GetOrAdd(
                wrapResultAttribute.NamingStrategyType, namingStrategyType =>
                    new AbpMvcContractResolver(_serviceProvider.GetRequiredService<IIocResolver>())
                    {
                        NamingStrategy =
                            (NamingStrategy)Activator.CreateInstance(namingStrategyType)
                    });
        }
    }
}