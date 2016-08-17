using System;
using System.Collections.Generic;

namespace Abp.Configuration.Startup
{
    public interface IValidationConfiguration
    {
        HashSet<Type> IgnoredTypes { get; }
    }
}