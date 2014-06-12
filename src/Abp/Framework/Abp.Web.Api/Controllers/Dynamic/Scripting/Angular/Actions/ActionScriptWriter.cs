using System.Text;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Angular.Actions
{
    internal abstract class ActionScriptWriter
    {
        protected readonly DynamicApiControllerInfo ControllerInfo;
        protected readonly DynamicApiActionInfo MethodInfo;

        protected ActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            ControllerInfo = controllerInfo;
            MethodInfo = methodInfo;
        }

        public virtual void WriteTo(StringBuilder script)
        {

        }
    }
}