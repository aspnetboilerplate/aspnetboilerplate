namespace Abp.WebApi.Controllers.Dynamic.Scripting.jQuery.Actions
{
    internal class HttpPutActionScriptProxyGenerator : HttpPostActionScriptProxyGenerator
    {
        public HttpPutActionScriptProxyGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {
        }
    }
}