using System.Linq;
using System.Reflection;

namespace Abp.Reflection
{
    public static class ProxyHelper
    {
        /// <summary>
        /// Returns dynamic proxy target object if this is a proxied object, otherwise returns the given object. 
        /// </summary>
        public static object UnProxy(object obj)
        {
            return Castle.DynamicProxy.ProxyUtil.GetUnproxiedInstance(obj);
        }
    }
}
