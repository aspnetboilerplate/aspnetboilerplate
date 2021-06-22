using System;
using System.Collections.Generic;
using AutoMapper;

namespace Abp.AutoMapper
{
    public class AbpAutoMapperConfiguration : IAbpAutoMapperConfiguration
    {
        public List<Action<IMapperConfigurationExpression>> Configurators { get; }

        [Obsolete("Automapper will remove static API. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/4667")]
        public bool UseStaticMapper { get; set; }

        public AbpAutoMapperConfiguration()
        {
#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once the UseStaticMapper property is removed
			UseStaticMapper = true;
#pragma warning restore CS0618 // Type or member is obsolete, this line will be removed once the UseStaticMapper property is removed
            Configurators = new List<Action<IMapperConfigurationExpression>>();
        }
    }
}