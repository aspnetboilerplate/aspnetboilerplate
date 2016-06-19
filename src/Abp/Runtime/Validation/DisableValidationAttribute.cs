using System;

namespace Abp.Runtime.Validation
{
    /// <summary>
    /// Can be added to a method to disable auto validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DisableValidationAttribute : Attribute
    {
        
    }
}