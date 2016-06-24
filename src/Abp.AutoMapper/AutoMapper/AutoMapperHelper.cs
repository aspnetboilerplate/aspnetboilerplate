using System;
using System.Reflection;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    internal static class AutoMapperHelper
    {
        public static void CreateMap(Type type, IConfiguration cfg)
        {
            CreateMap<AutoMapFromAttribute>(type, cfg);
            CreateMap<AutoMapToAttribute>(type, cfg);
            CreateMap<AutoMapAttribute>(type, cfg);
        }

        public static void CreateMap<TAttribute>(Type type, IConfiguration cfg)
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

                        cfg.CreateMap(type, targetType, MemberList.Source, "AbpAutoMapperModuleProfile");
                    }

                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.From))
                    {
                        cfg.CreateMap(targetType, type, MemberList.Source, "AbpAutoMapperModuleProfile");
                    }
                }
            }
        }
    }
}