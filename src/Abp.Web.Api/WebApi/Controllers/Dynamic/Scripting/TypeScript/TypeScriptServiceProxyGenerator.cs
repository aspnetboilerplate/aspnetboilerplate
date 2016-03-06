using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Web;
using Abp.WebApi.Controllers.Dynamic.Scripting.Angular;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.TypeScript
{
    internal class TypeScriptServiceProxyGenerator : ITransientDependency
    {
        private DynamicApiControllerInfo _controllerInfo;
        private HashSet<Type> _typesToBeDone = new HashSet<Type>();
        private HashSet<Type> _doneTypes = new HashSet<Type>();
        private string _servicePrefix = "";

        public string Generate(DynamicApiControllerInfo controllerInfo, string servicePrefix)
        {
            if (_servicePrefix != servicePrefix)
            {
                //if there is a change in servicePrefix, we need to generate the types again
                _servicePrefix = servicePrefix;
                _typesToBeDone = new HashSet<Type>();
                _doneTypes = new HashSet<Type>();
            }
            _controllerInfo = controllerInfo;

            var script = new StringBuilder();

            script.AppendLine("     export class " + _controllerInfo.ServiceName.Substring(_controllerInfo.ServiceName.IndexOf('/') + 1));
            script.AppendLine("     {");
            script.AppendLine("         static $inject = ['$http'];");
            script.AppendLine("         constructor(private $http: ng.IHttpService){");
            script.AppendLine("     }");
            foreach (var methodInfo in _controllerInfo.Actions.Values)
            {
                PrepareInputParameterTypes(methodInfo.Method);
                List<Type> newTypes = new List<Type>();
                var returnType = TypeScriptHelper.GetTypeContractName(methodInfo.Method.ReturnType, newTypes);
                this.AddNewTypesIfRequired(newTypes.ToArray());
                if (returnType == "void")
                {
                    script.AppendLine(string.Format("           public {0} = function ({1}): abp.IPromise ",
                        methodInfo.ActionName.ToCamelCase(), GetMethodInputParameter(methodInfo.Method)));
                    script.AppendLine("{");
                    script.AppendLine("                    var self = this;");
                    script.AppendLine("                    return self.$http(angular.extend({");
                    script.AppendLine("                        abp: true,");
                    script.AppendLine("                        url: abp.appPath + '" + ActionScriptingHelper.GenerateUrlWithParameters(_controllerInfo, methodInfo) + "',");
                    script.AppendLine("                        method: '" + methodInfo.Verb.ToString().ToUpper(CultureInfo.InvariantCulture) + "',");

                    if (methodInfo.Verb == HttpVerb.Get)
                    {
                        script.AppendLine("                        params: " + ActionScriptingHelper.GenerateBody(methodInfo));
                    }
                    else
                    {
                        script.AppendLine("                        data: JSON.stringify(" + ActionScriptingHelper.GenerateBody(methodInfo) + ")");
                    }

                    script.AppendLine("                    }, httpParams));");


                    script.AppendLine("}");


                }

                else
                {
                    script.AppendLine(string.Format("           public {0} = function ({1}): abp.IGenericPromise<{2}> ", methodInfo.ActionName.ToCamelCase(), 
                        GetMethodInputParameter(methodInfo.Method), returnType));
                    script.AppendLine("{");
                    script.AppendLine("                    var self = this;");
                    script.AppendLine("                    return self.$http(angular.extend({");
                    script.AppendLine("                        abp: true,");
                    script.AppendLine("                        url: abp.appPath + '" + ActionScriptingHelper.GenerateUrlWithParameters(_controllerInfo, methodInfo) + "',");
                    script.AppendLine("                        method: '" + methodInfo.Verb.ToString().ToUpper(CultureInfo.InvariantCulture) + "',");

                    if (methodInfo.Verb == HttpVerb.Get)
                    {
                        script.AppendLine("                        params: " + ActionScriptingHelper.GenerateBody(methodInfo));
                    }
                    else
                    {
                        script.AppendLine("                        data: JSON.stringify(" + ActionScriptingHelper.GenerateBody(methodInfo) + ")");
                    }

                    script.AppendLine("                    }, httpParams));");

                    script.AppendLine("}");
                }
                    

            }
            
            script.AppendLine("     }");

            script.AppendLine("angular.module('abp').service('abp.services." + _controllerInfo.ServiceName.Replace("/", ".") + "', abp.services." + _controllerInfo.ServiceName.Replace("/", ".")+");");


            while (_typesToBeDone != null && _typesToBeDone.Count > 0)
            {
                Type type = _typesToBeDone.First();

                script.AppendLine(GenerateTypeScript(type));
                _doneTypes.Add(type);
                _typesToBeDone.RemoveWhere(x => x == type);
            }
            return script.ToString();
        }
        protected string GetMethodInputParameter(MethodInfo methodInfo)
        {
            var script = new StringBuilder();
            
            List<Type> newTypes = new List<Type>();
            foreach (var parameter in methodInfo.GetParameters())
            {
                script.Append(string.Format("{0} : {1},", parameter.Name.ToCamelCase(), TypeScriptHelper.GetTypeContractName(parameter.ParameterType, newTypes)));
            }
            script.Append("httpParams?: any");
            this.AddNewTypesIfRequired(newTypes.ToArray());
            return script.ToString();

        }
        protected string GenerateTypeScript(Type type)
        {
            if (type.IsArray ||
                (type.IsGenericType && (typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())
                ))
                )
            {
                if (type.GetElementType() != null)
                {
                    this.AddNewTypesIfRequired(type.GetElementType());
                }
                return "";
            }

            if (type.IsGenericType && typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return "";
            }



            var myscript = new StringBuilder();
            List<Type> newTypes = new List<Type>();
            myscript.AppendLine("     export class " + TypeScriptHelper.GetTypeContractName(type, newTypes));
            myscript.AppendLine("         {");
            foreach (var property in type.GetProperties())
            {
                myscript.AppendLine(string.Format("{0} : {1} ;", property.Name.ToCamelCase(), TypeScriptHelper.GetTypeContractName(property.PropertyType, newTypes)));
            }
            this.AddNewTypesIfRequired(newTypes.ToArray());
            myscript.AppendLine("         }");
            return myscript.ToString();
        }

        private void AddNewTypesIfRequired(params Type[] newTypes)
        {
            foreach (var type in newTypes)
                if (this.CanAddToBeDone(type))
                    _typesToBeDone.Add(type);
        }

        protected void PrepareInputParameterTypes(MethodInfo methodInfo)
        {
            foreach (var parameter in methodInfo.GetParameters())
            {
                AddNewTypesIfRequired(parameter.ParameterType);
            }
        }

        protected void PrepareOutputParameterTypes(MethodInfo methodInfo)
        {
            if (this.CanAddToBeDone(methodInfo.ReturnType))
            {
                _typesToBeDone.Add(methodInfo.ReturnType);
            }
        }
        private bool CanAddToBeDone(Type type)
        {
            if (type == typeof(Task))
                return false;
            if (_typesToBeDone.Count(z => z.FullName == type.FullName) == 0 && !TypeScriptHelper.IsIgnorantType(type) && !TypeScriptHelper.IsBasicType(type) && _doneTypes.Count(z => z.FullName == type.FullName) == 0)
                return true;
            return false;
        }
    }
}
