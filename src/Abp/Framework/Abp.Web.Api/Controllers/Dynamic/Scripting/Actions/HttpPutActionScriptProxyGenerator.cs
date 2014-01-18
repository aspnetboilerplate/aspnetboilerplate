namespace Abp.WebApi.Controllers.Dynamic.Scripting.Actions
{
    internal class HttpPutActionScriptProxyGenerator : HttpPostActionScriptProxyGenerator
    {
        public HttpPutActionScriptProxyGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {
        }
    }
}