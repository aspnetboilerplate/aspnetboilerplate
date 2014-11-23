using System;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperHelper
    {
        public static void CreateMapForTypesInAssembly(Assembly assembly)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            if (types.IsNullOrEmpty())
            {
                return;
            }

            types = (
                from type in types
                where type != null &&
                      (type.IsDefined(typeof(AutoMapAttribute)) ||
                       type.IsDefined(typeof(AutoMapFromAttribute)) ||
                       type.IsDefined(typeof(AutoMapToAttribute)))
                select type
                ).ToArray();

            foreach (var type in types)
            {
                CreateMap<AutoMapFromAttribute>(type);
                CreateMap<AutoMapToAttribute>(type);
                CreateMap<AutoMapAttribute>(type);
            }
        }

        private static void CreateMap<TAttribute>(Type type)
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
                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.From))
                    {
                        Mapper.CreateMap(type, targetType);                                
                    }

                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.To))
                    {
                        Mapper.CreateMap(targetType, type);
                    }
                }
            }
        }
    }
}