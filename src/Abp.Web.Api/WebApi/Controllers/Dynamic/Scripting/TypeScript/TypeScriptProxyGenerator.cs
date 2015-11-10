using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.TypeScript
{
    internal class TypeScriptProxyGenerator : ITransientDependency
    {
        private DynamicApiControllerInfo _controllerInfo;
        readonly HashSet<Type> _typesToBeDone = new HashSet<Type>();
        private readonly HashSet<Type> _doneTypes = new HashSet<Type>();

        public string Generate(DynamicApiControllerInfo controllerInfo)
        {
            _controllerInfo = controllerInfo;

            var script = new StringBuilder();

            script.AppendLine("     interface " + _controllerInfo.ServiceName.Substring(_controllerInfo.ServiceName.IndexOf('/') + 1));
            script.AppendLine("     {");

            foreach (var methodInfo in _controllerInfo.Actions.Values)
            {
                PrepareInputParameterTypes(methodInfo.Method);
                PrepareOutputParameterTypes(methodInfo.Method);

                script.AppendLine(string.Format("            {0} ({1}): abp.IPromise<{2}>; ", methodInfo.ActionName.ToCamelCase(), GetMethodInputParameter(methodInfo.Method), GetTypeContractName(methodInfo.Method.ReturnType)));
            }

            script.AppendLine("     }");
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
            foreach (var parameter in methodInfo.GetParameters())
            {
                script.Append(string.Format("{0} : {1}", parameter.Name.ToCamelCase(), GetTypeContractName(parameter.ParameterType)));
            }
            return script.ToString();

        }
        protected void PrepareOutputParameterTypes(MethodInfo methodInfo)
        {
            if (!_typesToBeDone.Contains(methodInfo.ReturnType) && !IsBasicType(methodInfo.ReturnType) && !_doneTypes.Contains(methodInfo.ReturnType))
            {
                _typesToBeDone.Add(methodInfo.ReturnType);
            }
        }
        protected void PrepareInputParameterTypes(MethodInfo methodInfo)
        {
            foreach (var parameter in methodInfo.GetParameters())
            {
                if (!_typesToBeDone.Contains(parameter.ParameterType) && !IsBasicType(parameter.ParameterType))
                {
                    _typesToBeDone.Add(parameter.ParameterType);
                }
            }
        }
        protected void PrepareInputParameterTypes(Type type)
        {
            if (!_typesToBeDone.Contains(type) && !IsBasicType(type))
            {
                _typesToBeDone.Add(type);
            }

        }

        protected string GenerateTypeScript(Type type)
        {
            var myscript = new StringBuilder();

            myscript.AppendLine("     interface " + GetTypeContractName(type));
            myscript.AppendLine("         {");
            foreach (var property in type.GetProperties())
            {
                myscript.AppendLine(string.Format("{0} : {1} ;", property.Name.ToCamelCase(), GetTypeContractName(property.PropertyType)));
            }
            myscript.AppendLine("         }");
            return myscript.ToString();
        }
        protected string GenerateJsMethodParameterList(MethodInfo methodInfo)
        {
            var paramNames = methodInfo.GetParameters().Select(prm => prm.Name.ToCamelCase()).ToList();
            paramNames.Add("httpParams");
            return string.Join(", ", paramNames);
        }
        protected bool IsBasicType(Type type)
        {
            string[] input = { "guid", "string","bool",
                           "datetime","int16","int32", "int64","single","double","boolean","void"};

            List<string> basicTypes = new List<string>(input);
            if (basicTypes.Contains(type.Name.ToLowerInvariant()))
                return true;
            else
                return false;
        }

        private string GetTypeContractName(Type type)
        {
            if (type == typeof(Task))
            {
                return "void /*task*/";
            }

            if (type.IsArray)
            {
                return GetTypeContractName(type.GetElementType()) + "[]";
            }

            if (type.IsGenericType && (typeof(Task<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof(TaskFactory<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
            {
                return GetTypeContractName(type.GetGenericArguments()[0]);
            }

            if (type.IsGenericType && typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return GetTypeContractName(type.GetGenericArguments()[0]);
            }

            if (type.IsGenericType && (
                typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())
                ))
            {
                return GetTypeContractName(type.GetGenericArguments()[0]) + "[]";
            }
            if (type.IsEnum)
            {
                return "number";
            }
            switch (type.Name.ToLowerInvariant())
            {
                case "guid":
                    return "string";
                case "datetime":
                    return "string";
                case "int16":
                case "int32":
                case "int64":
                case "single":
                case "double":
                    return "number";
                case "boolean":
                case "bool":
                    return "boolean";
                case "void":
                case "string":
                    return type.Name.ToLowerInvariant();
            }
            if (!_typesToBeDone.Contains(type) && !_doneTypes.Contains(type))
            {
                _typesToBeDone.Add(type);
            }
            return GenericSpecificName(type).ToCamelCase();
        }

        string GenericSpecificName(Type type)
        {
            //todo: update for Typescript's generic syntax once invented
            string name = type.Name;
            int index = name.IndexOf('`');
            name = index == -1 ? name : name.Substring(0, index);
            if (type.IsGenericType)
            {
                name += "Of" + string.Join("And", type.GenericTypeArguments.Select(GenericSpecificName));
            }
            return name;
        }

    }
}
