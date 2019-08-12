using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Abp.Collections.Extensions;
using AutoMapper;
using AutoMapper.Collection;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Internal;
using Abp.Reflection.Extensions;

namespace Abp.AutoMapper
{
    public class AbpAutoMapToAttribute : AbpAutoMapAttributeBase
    {
        public MemberList MemberList { get; set; } = MemberList.Source;

        public AbpAutoMapToAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }

        public AbpAutoMapToAttribute(MemberList memberList, params Type[] targetTypes)
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

            configuration.CreateAutoAttributeMaps(type, TargetTypes, MemberList);
        }
    }
}