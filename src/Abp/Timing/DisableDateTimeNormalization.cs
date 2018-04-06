using System;

namespace Abp.Timing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DisableDateTimeNormalizationAttribute : Attribute
    {
        
    }
}