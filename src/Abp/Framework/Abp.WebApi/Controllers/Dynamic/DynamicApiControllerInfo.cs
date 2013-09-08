using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// Used to store controller type and name.
    /// </summary>
    internal class DynamicApiControllerInfo
    {
        /// <summary>
        /// Name of the controller.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Controller type.
        /// </summary>
        public Type Type { get; private set; }

        public Type ProxiedType { get; private set; }

        public IDictionary<string, MethodInfo> Methods { get; private set; }

        public DynamicApiControllerInfo(string name, Type type, Type proxiedType)
        {
            Name = name;
            Type = type;
            ProxiedType = proxiedType;
            Methods = new Dictionary<string, MethodInfo>();
            CacheMethods();
        }

        private void CacheMethods()
        {
            foreach (var method in ProxiedType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Methods.ContainsKey(method.Name))
                {
                    throw new ApplicationException("This service can not be proxied dynamically since it contains more than one definition for method " + method.Name);
                }

                Methods[method.Name] = method;
            }
        }
    }
}