using System;

namespace Abp.Timing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DisableDateTimeNormalizationAttribute : Attribute
    {
        
    }
}