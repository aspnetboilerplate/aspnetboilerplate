using System.Linq;
using System.Reflection;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.jQuery.Actions
{
    internal abstract class ActionScriptProxyGenerator
    {
        protected DynamicApiControllerInfo ControllerInfo { get; private set; }

        protected DynamicApiActionInfo ActionInfo { get; private set; }

        private const string JsMethodTemplate =
@"    serviceNamespace.{jsMethodName} = function({jsMethodParameterList}) {
        return abp.ajax($.extend({
{ajaxCallParameters}
        }, ajaxParams));
    };";

        protected ActionScriptProxyGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            ControllerInfo = controllerInfo;
            ActionInfo = methodInfo;
        }

        public virtual string GenerateMethod()
        {
            var jsMethodName = ActionInfo.ActionName.ToCamelCase();
            var jsMethodParameterList = GenerateJsMethodParameterList(ActionInfo.Method);

            var jsMethod = JsMethodTemplate
                .Replace("{jsMethodName}", jsMethodName)
                .Replace("{jsMethodParameterList}", jsMethodParameterList)
                .Replace("{ajaxCallParameters}", GenerateAjaxCallParameters());

            return jsMethod;
        }

        protected abstract string GenerateAjaxCallParameters();

        protected virtual string PlainActionUrl
        {
            get
            {
                return "api/services/" + ControllerInfo.ServiceName + "/" + ActionInfo.ActionName;
            }
        }

        protected string GenerateJsMethodParameterList(MethodInfo methodInfo)
        {
            var paramNames = methodInfo.GetParameters().Select(prm => prm.Name.ToCamelCase()).ToList();
            paramNames.Add("ajaxParams");
            return string.Join(", ", paramNames);
        }
    }
}