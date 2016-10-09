using System;
using System.Collections.Generic;
using AutoMapper;

namespace Abp.AutoMapper
{
    public interface IAbpAutoMapperConfiguration
    {
        List<Action<IMapperConfigurationExpression>> Configurators { get; }
    }
}