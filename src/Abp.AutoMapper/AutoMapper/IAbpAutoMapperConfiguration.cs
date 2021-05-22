using System;
using System.Collections.Generic;
using AutoMapper;

namespace Abp.AutoMapper
{
    public interface IAbpAutoMapperConfiguration
    {
        List<Action<IMapperConfigurationExpression>> Configurators { get; }

        /// <summary>
        /// Use static Instance.
        /// Default: true.
        /// </summary>
        [Obsolete("Automapper will remove static API. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/4667")]
        bool UseStaticMapper { get; set; }
    }
}