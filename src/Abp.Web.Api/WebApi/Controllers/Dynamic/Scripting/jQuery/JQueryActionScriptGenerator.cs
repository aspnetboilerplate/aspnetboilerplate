using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.jQuery
{
    internal class JQueryActionScriptGenerator
    {
        private readonly DynamicApiControllerInfo _controllerInfo;
        private readonly DynamicApiActionInfo _actionInfo;

        private const string JsMethodTemplate =
@"    serviceNamespace.{jsMethodName} = function({jsMethodParameterList}) {
        return abp.ajax($.extend({
{ajaxCallParameters}
        }, ajaxParams));
    };";

        private const string AjaxParametersTemplate =
@"            url: abp.appPath + '{url}',
            type: '{type}',
            data: JSON.stringify({postData})";

        public JQueryActionScriptGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo actionInfo)
        {
            _controllerInfo = controllerInfo;
            _actionInfo = actionInfo;
        }

        public virtual string GenerateMethod()
        {
            var jsMethodName = _actionInfo.ActionName.ToCamelCase();
            var jsMethodParameterList = ActionScriptingHelper.GenerateJsMethodParameterList(_actionInfo.Method, "ajaxParams");

            var jsMethod = JsMethodTemplate
                .Replace("{jsMethodName}", jsMethodName)
                .Replace("{jsMethodParameterList}", jsMethodParameterList)
                .Replace("{ajaxCallParameters}", GenerateAjaxCallParameters());

            return jsMethod;
        }

        protected string GenerateAjaxCallParameters()
        {
            var ajaxParameters = AjaxParametersTemplate
                .Replace("{url}", ActionScriptingHelper.GenerateUrlWithParameters(_controllerInfo, _actionInfo))
                .Replace("{type}", _actionInfo.Verb.ToString().ToUpperInvariant())
                .Replace("{postData}", ActionScriptingHelper.GenerateBody(_actionInfo));

            return ajaxParameters;
        }
    }
}