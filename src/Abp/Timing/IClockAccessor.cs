using System;
using Abp.Dependency;
using Microsoft.Extensions.Options;

namespace Abp.Timing
{
    public interface IClockAccessor : ISingletonDependency
    {
        DateTime Now { get; }
        DateTimeKind Kind { get; }
        bool SupportsMultipleTimezone { get; }
        DateTime Normalize(DateTime dateTime);
        IClockProvider Provider { get; set; }
    }
}