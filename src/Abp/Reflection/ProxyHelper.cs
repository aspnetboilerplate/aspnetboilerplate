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
            if (obj.GetType().Namespace != "Castle.Proxies")
            {
                return obj;
            }

            var targetField = obj.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(f => f.Name == "__target");

            if (targetField == null)
            {
                return obj;
            }

            return targetField.GetValue(obj);
        }
    }
}
