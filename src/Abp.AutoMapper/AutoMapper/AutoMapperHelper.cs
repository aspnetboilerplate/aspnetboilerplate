using System;
using System.Reflection;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperHelper
    {
        public static void CreateMap(IConfiguration configuration, Type type)
        {
            CreateMap<AutoMapFromAttribute>(configuration, type);
            CreateMap<AutoMapToAttribute>(configuration, type);
            CreateMap<AutoMapAttribute>(configuration, type);
        }

        public static void CreateMap<TAttribute>(IConfiguration configuration, Type type)
            where TAttribute : AutoMapAttribute
        {
            if (!type.IsDefined(typeof (TAttribute)))
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

        internal static void Initialize(Action<IConfiguration> action)
        {
            Mapper.Initialize(action);
        }
    }
}