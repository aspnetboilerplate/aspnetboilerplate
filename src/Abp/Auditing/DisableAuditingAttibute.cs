using System;

namespace Abp.Auditing
{
    /// <summary>
    /// Used to disable auditing for a single method or
    /// all methods of a class or interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class DisableAuditingAttibute : Attribute
    {

    }
}