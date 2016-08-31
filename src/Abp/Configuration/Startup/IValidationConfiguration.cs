using System;
using System.Collections.Generic;

namespace Abp.Configuration.Startup
{
    public interface IValidationConfiguration
    {
        List<Type> IgnoredTypes { get; }
    }
}