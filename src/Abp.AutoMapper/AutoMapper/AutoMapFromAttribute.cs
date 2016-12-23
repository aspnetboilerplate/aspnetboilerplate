using System;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    public class AutoMapFromAttribute : AutoMapAttributeBase
    {
        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            foreach (var targetType in TargetTypes)
            {
                configuration.CreateMap(targetType, type, MemberList.Destination);
            }
        }
    }
}