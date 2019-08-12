using System;
using Abp.Collections.Extensions;
using AutoMapper;

namespace Abp.AutoMapper
{
    public class AbpAutoMapFromAttribute : AbpAutoMapAttributeBase
    {
        public MemberList MemberList { get; set; } = MemberList.Destination;

        public AbpAutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }

        public AbpAutoMapFromAttribute(MemberList memberList, params Type[] targetTypes)
            : this(targetTypes)
        {
            MemberList = memberList;
        }

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            foreach (var targetType in TargetTypes)
            {
                configuration.CreateAutoAttributeMaps(targetType, new[] { type }, MemberList);
            }
        }
    }
}