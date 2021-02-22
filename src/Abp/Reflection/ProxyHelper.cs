using Castle.DynamicProxy;
using System;

namespace Abp.Reflection
{
    public static class ProxyHelper
    {
        /// <summary>
        /// Returns dynamic proxy target object if this is a proxied object, otherwise returns the given object. 
        /// </summary>
        public static object UnProxy(object obj)
        {
            return ProxyUtil.GetUnproxiedInstance(obj);
        }

        /// <summary>
        /// Returns the type of the dynamic proxy target object if this is a proxied object, otherwise returns the type of the given object.
        /// </summary>
        public static Type GetUnproxiedType(object obj)
        {
            return ProxyUtil.GetUnproxiedType(obj);
        }
    }
}
