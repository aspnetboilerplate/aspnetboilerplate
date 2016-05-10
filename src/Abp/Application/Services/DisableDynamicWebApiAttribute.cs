using System;

namespace Abp.Application.Services
{
    /// <summary>
    ///     Add this attribute to an application service interface or an interface method
    ///     to disable dynamic web api creation.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class DisableDynamicWebApiAttribute : Attribute
    {
    }
}