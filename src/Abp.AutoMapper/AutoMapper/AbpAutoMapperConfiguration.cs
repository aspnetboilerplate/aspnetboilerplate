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
            UseStaticMapper = true;
            Configurators = new List<Action<IMapperConfigurationExpression>>();
        }
    }
}