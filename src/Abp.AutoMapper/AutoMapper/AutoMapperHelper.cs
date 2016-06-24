using System;
using System.Reflection;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperHelper
    {
        public static void CreateMap(Type type, IConfiguration cfg = null)
        {
            CreateMap<AutoMapFromAttribute>(type, cfg);
            CreateMap<AutoMapToAttribute>(type, cfg);
            CreateMap<AutoMapAttribute>(type, cfg);
        }

        public static void CreateMap<TAttribute>(Type type, IConfiguration cfg = null)
            where TAttribute : AutoMapAttribute
        {
            if (!type.IsDefined(typeof(TAttribute)))
            {
                return;
            }

            Func<Type, Type, IMappingExpression> _createMap;

            if (cfg != null)
            {
                _createMap = cfg.CreateMap;
            }
            else
            {
                _createMap = Mapper.CreateMap;
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

                        _createMap(type, targetType);
                    }

                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.From))
                    {
                        _createMap(targetType, type);
                    }
                }
            }
        }
    }
}