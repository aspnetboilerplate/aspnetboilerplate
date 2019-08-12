using System;
using AutoMapper;

namespace Abp.AutoMapper
{
    public abstract class AbpAutoMapAttributeBase : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        protected AbpAutoMapAttributeBase(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }

        public abstract void CreateMap(IMapperConfigurationExpression configuration, Type type);
    }
}