using System.Text;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.jQuery.Actions
{
    internal class HttpDeleteActionScriptProxyGenerator : ActionScriptProxyGenerator
    {
        private const string AjaxParametersTemplate = 
@"            url: abp.appPath + '{url}',
            type: '{type}'";

        public HttpDeleteActionScriptProxyGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {
        }

        protected override string GenerateAjaxCallParameters()
        {
            var ajaxParameters = AjaxParametersTemplate
                .Replace("{url}", GenerateUrlWithParameters())
                .Replace("{type}", ActionInfo.Verb.ToString().ToUpperInvariant());

            return ajaxParameters;
        }

        private string GenerateUrlWithParameters()
        {

            var urlBuilder = new StringBuilder(PlainActionUrl);
            if (!PlainActionUrl.Contains("?"))
            {
                urlBuilder.Append("?");
            }

            var parameters = ActionInfo.Method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterInfo = parameters[i];
                if (i > 0)
                {
                    urlBuilder.Append("&");
                }

                urlBuilder.Append(parameterInfo.Name.ToCamelCase() + "=' + escape(" + parameterInfo.Name.ToCamelCase() + ") + '");
            }
            return urlBuilder.ToString();
        }
    }
}