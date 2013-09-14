namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    internal class HttpPutActionScriptProxyGenerator : HttpPostActionScriptProxyGenerator
    {
        public HttpPutActionScriptProxyGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {
        }
    }
}