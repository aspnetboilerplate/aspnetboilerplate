using System;
using System.Reflection;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperHelper
    {
        public static void CreateAbpAttributeMaps(this IMapperConfigurationExpression configuration, Type type)
        {
            configuration.CreateAbpAttributeMap<AutoMapFromAttribute>(type);
            configuration.CreateAbpAttributeMap<AutoMapToAttribute>(type);
            configuration.CreateAbpAttributeMap<AutoMapAttribute>(type);
        }

        private static void CreateAbpAttributeMap<TAttribute>(this IMapperConfigurationExpression configuration, Type type)
            where TAttribute : AutoMapAttribute
        {
            if (!type.IsDefined(typeof(TAttribute)))
            {
                return;
            }

            foreach (var autoMapToAttribute in type.GetCustomAttributes<TAttribute>())
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