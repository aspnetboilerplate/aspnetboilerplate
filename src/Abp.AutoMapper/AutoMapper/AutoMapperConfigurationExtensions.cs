using System;
using System.Reflection;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperConfigurationExtensions
    {
        public static void CreateAbpAttributeMaps(this IMapperConfigurationExpression configuration, Type type)
        {
            foreach (var autoMapToAttribute in type.GetCustomAttributes<AutoMapAttributeBase>())
            {
                if (autoMapToAttribute.TargetTypes.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var targetType in autoMapToAttribute.TargetTypes)
                {
                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.To))
                    {
                        configuration.CreateMap(type, targetType);
                    }

                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.From))
                    {
                        configuration.CreateMap(targetType, type);
                    }
                }
            }
        }
    }
}