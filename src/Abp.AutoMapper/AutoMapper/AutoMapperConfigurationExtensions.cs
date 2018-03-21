using System;
using System.Linq;
using System.Reflection;
using Abp.Configuration;
using Abp.Domain.Entities;
using Abp.Localization;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperConfigurationExtensions
    {
        public static void CreateAutoAttributeMaps(this IMapperConfigurationExpression configuration, Type type)
        {
            foreach (var autoMapAttribute in type.GetTypeInfo().GetCustomAttributes<AutoMapAttributeBase>())
            {
                autoMapAttribute.CreateMap(configuration, type);
            }
        }
    }
}