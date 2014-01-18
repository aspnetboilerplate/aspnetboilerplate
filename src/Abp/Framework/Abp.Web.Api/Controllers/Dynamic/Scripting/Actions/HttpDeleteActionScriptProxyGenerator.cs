using System.Text;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Actions
{
    internal class HttpDeleteActionScriptProxyGenerator : ActionScriptProxyGenerator
    {
        private const string AjaxPatametersTemplate = 
@"            url: '{url}',
            type: '{type}'";

        public HttpDeleteActionScriptProxyGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {
        }

        protected override string GenerateAjaxCallParameters()
        {
            var ajaxPatameters = AjaxPatametersTemplate
                .Replace("{url}", GenerateUrlWithParameters())
                .Replace("{type}", ActionInfo.Verb.ToString().ToUpperInvariant());

            return ajaxPatameters;
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