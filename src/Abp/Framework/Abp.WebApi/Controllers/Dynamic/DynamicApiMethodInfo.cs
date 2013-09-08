using System.Reflection;

namespace Abp.WebApi.Controllers.Dynamic
{
    internal class DynamicApiMethodInfo
    {
        public MethodInfo Method { get; set; }

        public HttpVerb Verb { get; set; }

        public DynamicApiMethodInfo(MethodInfo method)
        {
            Method = method;
        }
    }
}