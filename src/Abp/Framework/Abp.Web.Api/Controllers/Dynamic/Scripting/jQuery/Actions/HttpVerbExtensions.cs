using Abp.Web;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.jQuery.Actions
{
    internal static class HttpVerbExtensions
    {
        public static ActionScriptProxyGenerator CreateActionScriptProxyGenerator(this HttpVerb verb, DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            switch (verb)
            {
                case HttpVerb.Get:
                    return new HttpGetActionScriptProxyGenerator(controllerInfo, methodInfo);
                case HttpVerb.Post:
                    return new HttpPostActionScriptProxyGenerator(controllerInfo, methodInfo);
                case HttpVerb.Put:
                    return new HttpPutActionScriptProxyGenerator(controllerInfo, methodInfo);
                case HttpVerb.Delete:
                    return new HttpDeleteActionScriptProxyGenerator(controllerInfo, methodInfo);
                default:
                    throw new AbpException("This Http verb is not implemented: " + verb);
            }
        }
    }
}