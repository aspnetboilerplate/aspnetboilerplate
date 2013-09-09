using System.Reflection;

namespace Abp.WebApi.Controllers.Dynamic
{
    internal class DynamicApiMethodInfo
    {
        /// <summary>
        /// Name of the method.
        /// </summary>
        public string Name { get; private set; }

        public MethodInfo Method { get; set; }

        public HttpVerb Verb { get; set; }

        public DynamicApiMethodInfo(string name, MethodInfo method, HttpVerb verb)
        {
            Name = name;
            Method = method;
            Verb = verb;
        }
    }
}