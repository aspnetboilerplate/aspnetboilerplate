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

    /// <summary>
    /// Can be added to a method to enable auto validation if validation is disabled for it's class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EnableValidationAttribute : Attribute
    {

    }
}