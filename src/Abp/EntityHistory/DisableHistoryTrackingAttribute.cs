using System;

namespace Abp.EntityHistory
{
    /// <summary>
    /// Used to disable history tracking for a class or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DisableHistoryTrackingAttribute : Attribute
    {

    }
}
