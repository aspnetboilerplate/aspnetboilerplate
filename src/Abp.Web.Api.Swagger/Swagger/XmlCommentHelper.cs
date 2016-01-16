using Abp.WebApi.Controllers.Dynamic;
using NJsonSchema;
using NSwag;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;
// from Swashbuckle.Core

namespace Abp.Web.Api.Swagger
{
    public static class XmlCommentHelper
    {
        private static Regex ParamPattern = new Regex(@"<(see|paramref) (name|cref)=""([TPF]{1}:)?(?<display>.+?)"" />");
        private static Regex ConstPattern = new Regex(@"<c>(?<display>.+?)</c>");


        public static void ApplyXmlTypeComments(JsonSchema4 schema, Dictionary<string, Type> find)
        {
            if (!find.Keys.Contains(schema.TypeName)) return;

            var path = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + find[schema.TypeName].Assembly.FullName.Split(',')[0] + ".XML";
            if (!File.Exists(path)) return;
            XPathNavigator _navigator = new XPathDocument(path).CreateNavigator();
            var typeNode = _navigator.SelectSingleNode(
                String.Format("/doc/members/member[@name='T:{0}']", find[schema.TypeName].XmlLookupName()));

            if (typeNode != null)
            {
                var summaryNode = typeNode.SelectSingleNode("summary");
                if (summaryNode != null)
                    schema.Description = summaryNode.ExtractContent();
            }
            var props = find[schema.TypeName].GetProperties();
            foreach (var entry in schema.Properties)
            {
                var jsonProperty = props.Where(p => p.Name == entry.Key).FirstOrDefault();
                if (jsonProperty == null) continue;

                ApplyPropertyComments(entry.Value, jsonProperty);
            }
        }
        private static void ApplyPropertyComments(JsonSchema4 propertySchema, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return;

            var path = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + propertyInfo.DeclaringType.Assembly.FullName.Split(',')[0] + ".XML";
            if (!File.Exists(path)) return;
            XPathNavigator _navigator = new XPathDocument(path).CreateNavigator();
            var propertyNode = _navigator.SelectSingleNode(
                String.Format("/doc/members/member[@name='P:{0}.{1}']", propertyInfo.DeclaringType.XmlLookupName(), propertyInfo.Name));
            if (propertyNode == null) return;

            var propSummaryNode = propertyNode.SelectSingleNode("summary");
            if (propSummaryNode != null)
            {
                propertySchema.Description = propSummaryNode.ExtractContent();
            }
        }

        public static void ApplyXMLComment(DynamicApiControllerInfo info, DynamicApiActionInfo actioninfo, SwaggerOperation operation)
        {

            var path = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + info.ServiceInterfaceType.Assembly.FullName.Split(',')[0] + ".XML";
            if (!File.Exists(path)) return;
            XPathNavigator _navigator = new XPathDocument(path).CreateNavigator();
            var methodNode = _navigator.SelectSingleNode(XmlCommentHelper.XPathFor(info, actioninfo));
            if (methodNode == null) return;

            var summaryNode = methodNode.SelectSingleNode("summary");
            if (summaryNode != null)
                operation.Summary = summaryNode.ExtractContent();

            var remarksNode = methodNode.SelectSingleNode("remarks");
            if (remarksNode != null)
                operation.Description = remarksNode.ExtractContent();

            ApplyParamComments(operation, methodNode);

            ApplyResponseComments(operation, methodNode);
        }

        static void ApplyResponseComments(SwaggerOperation operation, XPathNavigator methodNode)
        {
            var responseNodes = methodNode.Select("response");

            if (responseNodes.Count > 0)
            {
                var successResponse = operation.Responses.First().Value;
                operation.Responses.Clear();

                while (responseNodes.MoveNext())
                {
                    var statusCode = responseNodes.Current.GetAttribute("code", "");
                    var description = responseNodes.Current.ExtractContent();

                    var response = new SwaggerResponse
                    {
                        Description = description,
                        Schema = statusCode.StartsWith("2") ? successResponse.Schema : null
                    };
                    operation.Responses[statusCode] = response;
                }
            }
        }

        static void ApplyParamComments(SwaggerOperation operation, XPathNavigator methodNode)
        {
            if (operation.Parameters == null) return;

            var paramNodes = methodNode.Select("param");
            while (paramNodes.MoveNext())
            {
                var paramNode = paramNodes.Current;
                var parameter = operation.Parameters.SingleOrDefault(param => param.Name == paramNode.GetAttribute("name", ""));
                if (parameter != null)
                    parameter.Description = paramNode.ExtractContent();
            }
        }

        public static string ExtractContent(this XPathNavigator node)
        {
            if (node == null) return null;

            return ConstPattern.Replace(
                ParamPattern.Replace(node.InnerXml, GetParamRefName),
                GetConstRefName).Trim();
        }

        private static string GetConstRefName(Match match)
        {
            if (match.Groups.Count != 2) return null;

            return match.Groups["display"].Value;
        }

        private static string GetParamRefName(Match match)
        {
            if (match.Groups.Count != 5) return null;

            return "{" + match.Groups["display"].Value + "}";
        }
        public static string XPathFor(DynamicApiControllerInfo info,DynamicApiActionInfo actioninfo)
        {
            var controllerName = info.ServiceInterfaceType.XmlLookupName();            
            var actionName = actioninfo.ActionName;

            var paramTypeNames = actioninfo.Method.GetParameters()
                .Select(paramDesc => paramDesc.ParameterType.XmlLookupNameWithTypeParameters())
                .ToArray();

            var parameters = (paramTypeNames.Any())
                ? String.Format("({0})", String.Join(",", paramTypeNames))
                : String.Empty;

            return String.Format("/doc/members/member[@name='M:{0}.{1}{2}']", controllerName, actionName, parameters);
        }

        public static string XmlLookupName(this Type type)
        {
            var builder = new StringBuilder(type.FullNameSansTypeParameters());
            return builder
                .Replace("+", ".")
                .ToString();
        }

        public static string XmlLookupNameWithTypeParameters(this Type type)
        {
            var builder = new StringBuilder(type.XmlLookupName());

            if (type.IsGenericType)
            {
                var typeParameterQualifiers = type.GetGenericArguments()
                    .Select(t => t.XmlLookupNameWithTypeParameters())
                    .ToArray();

                builder
                    .Replace(String.Format("`{0}", typeParameterQualifiers.Count()), String.Empty)
                    .Append(String.Format("{{{0}}}", String.Join(",", typeParameterQualifiers).TrimEnd(',')))
                    .ToString();
            }

            return builder.ToString();
        }

        public static string FriendlyId(this Type type, bool fullyQualified = false)
        {
            var typeName = fullyQualified
                ? type.FullNameSansTypeParameters().Replace("+", ".")
                : type.Name;

            if (type.IsGenericType)
            {
                var genericArgumentIds = type.GetGenericArguments()
                    .Select(t => t.FriendlyId(fullyQualified))
                    .ToArray();

                return new StringBuilder(typeName)
                    .Replace(string.Format("`{0}", genericArgumentIds.Count()), string.Empty)
                    .Append(string.Format("[{0}]", string.Join(",", genericArgumentIds).TrimEnd(',')))
                    .ToString();
            }

            return typeName;
        }
        public static string FullNameSansTypeParameters(this Type type)
        {
            var fullName = type.FullName;
            if (string.IsNullOrEmpty(fullName))
                fullName = type.Name;
            var chopIndex = fullName.IndexOf("[[");
            return (chopIndex == -1) ? fullName : fullName.Substring(0, chopIndex);
        }
    }
}
