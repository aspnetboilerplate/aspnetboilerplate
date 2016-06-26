using System.Linq;
using System.Text;
using Abp.Extensions;
using Abp.Reflection;
using Abp.Web.Api.Modeling;

namespace Abp.Web.Api.ProxyScripting.Generators.JQuery
{
    internal static class ProxyScriptingHelper
    {
        public static string GenerateUrlWithParameters(ActionApiDescriptionModel action)
        {
            var baseUrl = action.Url;

            var primitiveParameters = action.Parameters
                .Where(p => TypeHelper.IsPrimitiveExtendedIncludingNullable(p.Type))
                .ToArray();

            if (!primitiveParameters.Any())
            {
                return baseUrl;
            }

            var urlBuilder = new StringBuilder(baseUrl);
            if (!baseUrl.Contains("?"))
            {
                urlBuilder.Append("?");
            }

            for (var i = 0; i < primitiveParameters.Length; i++)
            {
                var parameterInfo = primitiveParameters[i];

                if (i > 0)
                {
                    urlBuilder.Append("&");
                }

                urlBuilder.Append(parameterInfo.Name.ToCamelCase() + "=' + escape(" + parameterInfo.Name.ToCamelCase() + ") + '");
            }

            return urlBuilder.ToString();
        }

        public static string GenerateJsMethodParameterList(ActionApiDescriptionModel action, string ajaxParametersName)
        {
            var paramNames = action.Parameters.Select(prm => prm.Name.ToCamelCase()).ToList();
            paramNames.Add(ajaxParametersName);
            return string.Join(", ", paramNames);
        }

        public static string GenerateBody(ActionApiDescriptionModel action)
        {
            var parameters = action
                .Parameters
                .Where(p => !TypeHelper.IsPrimitiveExtendedIncludingNullable(p.Type))
                .ToArray();

            if (parameters.Length <= 0)
            {
                return "{}";
            }

            if (parameters.Length > 1)
            {
                //TODO: More detailed message, including type
                throw new AbpException("Only one complex type allowed as argument to a web api controller action. But " + action.Name + " contains more than one!");
            }

            return parameters[0].Name.ToCamelCase();
        }
    }
}
