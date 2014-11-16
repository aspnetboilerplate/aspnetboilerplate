using System;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperHelper
    {
        public static void AutoMapInAssembly(Assembly assembly)
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
                where type.IsClass && type.IsDefined(typeof(AutoMapAttribute))
                select type
                ).ToArray();

            foreach (var sourceType in types)
            {
                foreach (var autoMapAttr in sourceType.GetCustomAttributes<AutoMapAttribute>())
                {
                    foreach (var targetType in autoMapAttr.TargetTypes)
                    {
                        Mapper.CreateMap(sourceType, targetType);
                        Mapper.CreateMap(targetType, sourceType);
                    }
                }
            }
        }
    }
}