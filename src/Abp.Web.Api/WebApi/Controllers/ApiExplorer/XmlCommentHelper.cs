using Abp.WebApi.Controllers.Dynamic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.Description;
using System.Xml.XPath;

namespace Abp.WebApi.Controllers.ApiExplorer
{
    public static class XmlCommentHelper
    {
        private static Regex ParamPattern = new Regex(@"<(see|paramref) (name|cref)=""([TPF]{1}:)?(?<display>.+?)"" />");
        private static Regex ConstPattern = new Regex(@"<c>(?<display>.+?)</c>");



        internal static void ApplyXMLComment(DynamicApiControllerInfo info, DynamicApiActionInfo actioninfo, ApiDescription api)
        {
            if (!string.IsNullOrEmpty(api.Documentation)) return;
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + info.ServiceInterfaceType.Assembly.FullName.Split(',')[0] + ".XML";
            if (!File.Exists(path)) return;
            XPathNavigator _navigator = new XPathDocument(path).CreateNavigator();
            var methodNode = _navigator.SelectSingleNode(XmlCommentHelper.XPathFor(info, actioninfo));
            if (methodNode == null) return;

            var summaryNode = methodNode.SelectSingleNode("summary");
            if (summaryNode != null)
                api.Documentation = summaryNode.ExtractContent();

            var remarksNode = methodNode.SelectSingleNode("remarks");
            if (remarksNode != null)
                api.Documentation+= remarksNode.ExtractContent();

            ApplyParamComments(api, methodNode);

        }

     

        static void ApplyParamComments(ApiDescription api, XPathNavigator methodNode)
        {
            if (api.ParameterDescriptions == null) return;

            var paramNodes = methodNode.Select("param");
            while (paramNodes.MoveNext())
            {
                var paramNode = paramNodes.Current;
                var parameter = api.ParameterDescriptions.Where(p => p.Name == paramNode.GetAttribute("name","") && string.IsNullOrEmpty(p.Documentation)).FirstOrDefault();
                if (parameter != null)
                    parameter.Documentation = paramNode.ExtractContent();
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
        internal static string XPathFor(DynamicApiControllerInfo info, DynamicApiActionInfo actioninfo)
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
