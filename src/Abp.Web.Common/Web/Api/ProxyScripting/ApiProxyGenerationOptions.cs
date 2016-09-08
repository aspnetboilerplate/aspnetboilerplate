using System.Collections.Generic;
using Abp.Collections.Extensions;

namespace Abp.Web.Api.ProxyScripting
{
    public class ApiProxyGenerationOptions
    {
        public string GeneratorType { get; set; }

        public bool UseCache { get; set; }

        public string[] Modules { get; set; }

        public string[] Controllers { get; set; }

        public string[] Actions { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public ApiProxyGenerationOptions(string generatorType, bool useCache = true)
        {
            GeneratorType = generatorType;
            UseCache = useCache;

            Properties = new Dictionary<string, string>();
        }

        public bool IsPartialRequest()
        {
            return !(Modules.IsNullOrEmpty() && Controllers.IsNullOrEmpty() && Actions.IsNullOrEmpty());
        }
    }
}