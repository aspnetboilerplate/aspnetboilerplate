using System;
using System.Reflection;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperConfigurationExtensions
    {
        public static void CreateAutoAttributeMaps(this IMapperConfigurationExpression configuration, Type type)
        {
            foreach (var autoMapAttribute in type.GetCustomAttributes<AutoMapAttributeBase>())
            {
                autoMapAttribute.CreateMap(configuration, type);
            }
        }
    }
}