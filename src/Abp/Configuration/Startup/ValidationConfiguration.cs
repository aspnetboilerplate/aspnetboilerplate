using System;
using System.Collections.Generic;

namespace Abp.Configuration.Startup
{
    public class ValidationConfiguration : IValidationConfiguration
    {
        public HashSet<Type> IgnoredTypes { get; }

        public ValidationConfiguration()
        {
            IgnoredTypes = new HashSet<Type>();
        }
    }
}