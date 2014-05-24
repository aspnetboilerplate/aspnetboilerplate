using System;
using System.Text.RegularExpressions;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    internal static class DynamicApiServiceNameHelper
    {
        private static readonly Regex ServiceNameRegex = new Regex(@"^([a-zA-Z_][a-zA-Z0-9_]*)(\/([a-zA-Z_][a-zA-Z0-9_]*))+$");
        private static readonly Regex ServiceNameWithActionRegex = new Regex(@"^([a-zA-Z_][a-zA-Z0-9_]*)(\/([a-zA-Z_][a-zA-Z0-9_]*)){2,}$");

        public static bool IsValidServiceName(string serviceName)
        {
            return ServiceNameRegex.IsMatch(serviceName);
        }

        public static bool IsValidServiceNameWithAction(string serviceNameWithAction)
        {
            return ServiceNameWithActionRegex.IsMatch(serviceNameWithAction);
        }

        public static string GetServiceNameInServiceNameWithAction(string serviceNameWithAction)
        {
            return serviceNameWithAction.Substring(0, serviceNameWithAction.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase));
        }

        public static string GetActionNameInServiceNameWithAction(string serviceNameWithAction)
        {
            return serviceNameWithAction.Substring(serviceNameWithAction.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1);
        }
    }
}
