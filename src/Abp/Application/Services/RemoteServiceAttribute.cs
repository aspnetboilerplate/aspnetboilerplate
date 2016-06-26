using System;
using System.Reflection;

namespace Abp.Application.Services
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
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
    }
}