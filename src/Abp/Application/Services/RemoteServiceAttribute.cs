using System;
using System.Reflection;
using Abp.Reflection.Extensions;

namespace Abp.Application.Services
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public class RemoteServiceAttribute : Attribute
    {
        public bool IsEnabled { get; set; }

        public RemoteServiceAttribute(bool isEnabled = true)
        {
            IsEnabled = isEnabled;
        }

        public virtual bool IsEnabledFor(Type type)
        {
            return IsEnabled;
        }

        public virtual bool IsEnabledFor(MethodInfo method)
        {
            return IsEnabled;
        }

        public static bool IsExplicitlyEnabledFor(Type type)
        {
            var remoteServiceAttr = type.GetSingleAttributeOrNull<RemoteServiceAttribute>();
            return remoteServiceAttr != null && remoteServiceAttr.IsEnabledFor(type);
        }

        public static bool IsExplicitlyDisabledFor(Type type)
        {
            var remoteServiceAttr = type.GetSingleAttributeOrNull<RemoteServiceAttribute>();
            return remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(type);
        }
    }
}