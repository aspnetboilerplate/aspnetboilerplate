using System.Linq;
using System.Text;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.TypeScript
{
    public class TypeScriptDefinitionGenerator : ITransientDependency
    {
        private readonly DynamicApiControllerManager _dynamicApiControllerManager;

        public TypeScriptDefinitionGenerator(DynamicApiControllerManager dynamicApiControllerManager)
        {
            _dynamicApiControllerManager = dynamicApiControllerManager;
        }

        public string GetScript()
        {
            var dynamicControllers = _dynamicApiControllerManager.GetAll();
            
            StringBuilder script = new StringBuilder();
            if (dynamicControllers == null || dynamicControllers.Count == 0)
                return "";
            //sorting the controllers and use this sorting for detecting the servicePrefix change
            //we create module per servicePrefix
            var sortedDynamicControllers = dynamicControllers.OrderBy(z => z.ServiceName);
            var servicePrefix = GetServicePrefix(sortedDynamicControllers.First().ServiceName);
            if (servicePrefix.IsNullOrEmpty())
                script.AppendLine("declare module abp.services");//Create a new Module
            else
                script.AppendLine("declare module abp.services." + servicePrefix);//Create a new Module
            script.AppendLine("{");
            var proxyGenerator = new TypeScriptDefinitionProxyGenerator();
            foreach (var dynamicController in sortedDynamicControllers)
            {
                if (servicePrefix != GetServicePrefix(dynamicController.ServiceName))
                {
                    //the service Prefix has been changed
                    servicePrefix = GetServicePrefix(dynamicController.ServiceName);
                    script.AppendLine("}");//Close the Previous Module
                    //Create new module for the new service prefix
                    if(servicePrefix.IsNullOrEmpty())
                        script.AppendLine("declare module abp.services");//Create a new Module
                    else
                        script.AppendLine("declare module abp.services." + servicePrefix);//Create a new Module
                    script.AppendLine("{");
                }
                script.AppendLine(proxyGenerator.Generate(dynamicController,servicePrefix));
                script.AppendLine();
            }
            script.AppendLine("}");
            #region Create Script for Abp common objects

            script.AppendLine("declare module abp {");
            script.AppendLine("class ui {");
            script.AppendLine("static setBusy(element, IPromise);");
            script.AppendLine("}");
            script.AppendLine("class nav {");
            script.AppendLine("static menus: any;");
            script.AppendLine("}");
            script.AppendLine("class message{");
            script.AppendLine("static info(message: string, title: string);");
            script.AppendLine("static success(message: string, title: string);");
            script.AppendLine("static warn(message: string, title: string);");
            script.AppendLine("static error(message: string, title: string);");
            script.AppendLine("}");
            script.AppendLine("class notify {");
            script.AppendLine("static info(message: string, title?: string);");
            script.AppendLine("static success(message: string, title?: string);");
            script.AppendLine("static warn(message: string, title?: string);");
            script.AppendLine("static error(message: string, title?: string);");
            script.AppendLine("}");
            script.AppendLine("class localization{");
            script.AppendLine("static languages: any;");
            script.AppendLine("static currentLanguage: any;");
            script.AppendLine("}");
            script.AppendLine("interface IGenericPromise<T> {");
            script.AppendLine("success(successCallback: (promiseValue: T) => any) : any;");
            script.AppendLine("error(errorCallback: () => any) : any;");
            script.AppendLine("}");
            script.AppendLine("interface IPromise {");
            script.AppendLine("success(successCallback: () => any) : any;");
            script.AppendLine("error(errorCallback: () => any) : any;");
            script.AppendLine("}");
            script.AppendLine("}");

            #endregion

            return script.ToString();
        }

        private string GetServicePrefix(string serviceName)
        {
            if (serviceName.IndexOf('/') == -1)
                return  "";
            else
                return serviceName.Substring(0,serviceName.IndexOf('/'));
        }
    }
}
