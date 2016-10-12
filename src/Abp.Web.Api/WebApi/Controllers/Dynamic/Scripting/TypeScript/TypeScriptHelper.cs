using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.TypeScript
{
    internal class TypeScriptHelper
    {
        
        private static readonly string[] _basicTypes =
        {
            "guid", "string", "bool",
            "datetime", "int16", "int32", "int64", "single", "double", "decimal", "boolean", "void","byte"
        };

        private static readonly string[] _typesToIgnore =
        {
            "exception", "aggregateexception","module","object"
        };

        public static bool IsBasicType(Type type)
        {
            if (_basicTypes.Contains(type.Name.ToLowerInvariant()))
                return true;
            else
                return false;
        }

        public static bool IsIgnorantType(Type type)
        {
            if (_typesToIgnore.Contains(type.Name.ToLowerInvariant()))
                return true;
            else
                return false;
        }

        public static string GetTypeContractName(Type type, List<Type> newTypesToAdd)
        {
            if (IsIgnorantType(type))
                return "any";

            if (type == typeof (Task))
            {
                return "void";
            }

            if (type.IsArray)
            {

                return GetTypeContractName(type.GetElementType(), newTypesToAdd) + "[]";
            }

            if (type.IsGenericType && (typeof (Task<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                                       typeof (TaskFactory<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
            {
                return GetTypeContractName(type.GetGenericArguments()[0], newTypesToAdd);
            }

            if (type.IsGenericType && typeof (Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return GetTypeContractName(type.GetGenericArguments()[0], newTypesToAdd);
            }

            if (type.IsGenericType && (
                typeof (List<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof (ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof (IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())
                ))
            {
                return GetTypeContractName(type.GetGenericArguments()[0], newTypesToAdd) + "[]";
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
                    return "Date";
                case "int16":
                case "int32":
                case "int64":
                case "single":
                case "double":
                case "decimal":
                case "byte":
                    return "number";
                case "boolean":
                case "bool":
                    return "boolean";
                case "void":
                case "string":
                    return type.Name.ToLowerInvariant();
            }

            newTypesToAdd.Add(type);

            return GenericSpecificName(type).ToCamelCase();
        }

        public static string GenericSpecificName(Type type)
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

        //todo: no use will be removed
        public static string GenerateJsMethodParameterList(MethodInfo methodInfo)
        {
            var paramNames = methodInfo.GetParameters().Select(prm => prm.Name.ToCamelCase()).ToList();
            paramNames.Add("httpParams");
            return string.Join(", ", paramNames);
        }
    }
}