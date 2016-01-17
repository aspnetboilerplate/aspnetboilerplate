using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Text.RegularExpressions;
using Abp.WebApi.Swagger.Adapter;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag;

namespace Abp.WebApi.Swagger.Builders
{
    /// <summary>
    /// Abp Swaggger Generator
    /// </summary>
    internal class AbpSwaggerGenerator
    {
        private AbpSwaggerService _service;
        private string _serviceName;

        /// <summary>Initializes a new instance of the <see cref="AbpSwaggerGenerator" /> class.</summary>
        /// <param name="defaultRouteTemplate">The default route template.</param>
        /// <param name="jsonSchemaGeneratorSettings">The JSON Schema generator settings.</param>
        public AbpSwaggerGenerator(string defaultRouteTemplate, JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings)
        {
            DefaultRouteTemplate = defaultRouteTemplate;
            JsonSchemaGeneratorSettings = jsonSchemaGeneratorSettings;
        }

        /// <summary>Gets or sets the default route template which is used when no route attributes are found (default: 'api/{controller}/{action}/{id}').</summary>
        public string DefaultRouteTemplate { get; set; }

        /// <summary>Gets or sets the JSON Schema generator settings.</summary>
        public JsonSchemaGeneratorSettings JsonSchemaGeneratorSettings { get; set; }

        /// <summary>
        /// Use Abp application service to structure abp swagger service
        /// </summary>
        /// <param name="serviceName">Application Service Name</param>
        /// <param name="type">Interface Type of your Application Service</param>
        /// <param name="checkOpenWebApi">
        /// Retain parameters, if you don't want open some action, just don't add OpenWebApiAttribute to your action method.
        /// But you must create this attribute.
        /// </param>
        /// <param name="excludedMethodName">Which method you don't want including to swagger document.</param>
        /// <returns></returns>
        public AbpSwaggerService GenerateForAbpAppService(string serviceName, Type type, bool checkOpenWebApi = false,
            string excludedMethodName = "Swagger")
        {
            _service = new AbpSwaggerService();
            _serviceName = serviceName;

            var schemaResolver = new SchemaResolver();
            var methods = type.GetMethods();

            dynamic webApiDesc = type.GetCustomAttributes()
                .FirstOrDefault(x => x.GetType().Name == "WebApiDescriptionAttribute");

            dynamic description = null;
            dynamic name = null;
            if (webApiDesc != null)
            {
                name = webApiDesc.Name;
                description = webApiDesc.Description;

                _service.Tags.Add(new AbpSwaggerTag
                {
                    Name = name,
                    Description = description
                });
            }

            _service.Info = new SwaggerInfo
            {
                Title = name,
                Version = description
            };

            var hasGenerated = false;
            foreach (var method in methods.Where(m => m.Name != excludedMethodName))
            {
                var canOpen = method.GetCustomAttributes().Any(x => x.GetType().Name == "OpenWebApiAttribute");
                if (!canOpen && checkOpenWebApi)
                    continue;

                var driveMethod = method; //GetSpecifiedMethod(deriveMethods, method);
                var parameters = method.GetParameters().ToList();
                var methodName = method.Name;

                var operation = new AbpSwaggerOperation { OperationId = methodName };

                dynamic webApiMethodDesc = method.GetCustomAttributes().FirstOrDefault(x => x.GetType().Name == "WebApiDescriptionAttribute");
                if (webApiMethodDesc != null)
                {
                    operation.Tags.Add(webApiMethodDesc.Name);
                }

                var httpPath = GetHttpPath(type, operation, driveMethod, parameters, schemaResolver);

                LoadParameters(operation, parameters, schemaResolver);

                LoadReturnType(operation, driveMethod, schemaResolver);

                LoadMetaData(operation, method);

                var httpMethod = GetMethod(method);

                if (!_service.Paths.ContainsKey(httpPath))
                {
                    var path = new AbpSwaggerOperations();
                    _service.Paths[httpPath] = path;
                }

                _service.Paths[httpPath][httpMethod] = operation;

                hasGenerated = true;
            }

            foreach (var schema in schemaResolver.Schemes)
            {
                _service.Definitions[schema.TypeName] = schema;
            }

            _service.GenerateOperationIds();

            if (!hasGenerated)
                _service = null;

            return _service;
        }


        private MethodInfo GetSpecifiedMethod(IEnumerable<MethodInfo> driveMethods, MethodInfo interfaceMethod)
        {
            var matchingMethods = driveMethods.Where(mi =>
               mi.ReturnType == interfaceMethod.ReturnType
               && mi.Name == interfaceMethod.Name
               && mi.GetParameters().Select(pi => pi.ParameterType)
                  .SequenceEqual(interfaceMethod.GetParameters().Select(pi => pi.ParameterType)));

            return matchingMethods.FirstOrDefault();
        }

        private void LoadMetaData(AbpSwaggerOperation operation, MethodInfo method)
        {
            dynamic descriptionAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "WebApiDescriptionAttribute");

            if (descriptionAttribute != null)
                operation.Summary = descriptionAttribute.Description;
            else
            {
                var summary = method.GetXmlDocumentation();
                if (summary != string.Empty)
                    operation.Summary = summary;
            }
        }

        private string GetHttpPath(Type serviceType, AbpSwaggerOperation operation, MethodInfo method, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            string httpPath;

            dynamic routeAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "RouteAttribute");

            if (routeAttribute != null)
            {
                dynamic routePrefixAttribute = serviceType.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "RoutePrefixAttribute");

                if (routePrefixAttribute != null)
                    httpPath = routePrefixAttribute.Prefix + "/" + routeAttribute.Template;
                else
                    httpPath = routeAttribute.Template;
            }
            else
            {
                httpPath = DefaultRouteTemplate
                    .Replace("{controller}", _serviceName)
                    .Replace("{action}", method.Name);
            }

            foreach (var match in Regex.Matches(httpPath, "\\{(.*?)\\}").OfType<Match>())
            {
                var parameter = parameters.SingleOrDefault(p => p.Name == match.Groups[1].Value);
                if (parameter != null)
                {
                    var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
                    operationParameter.Kind = AbpSwaggerParameterKind.Path;

                    operation.Parameters.Add(operationParameter);
                    parameters.Remove(parameter);
                }
                else
                {
                    httpPath = "/" + httpPath
                        .Replace(match.Value, string.Empty)
                        .Replace("//", "/")
                        .Trim('/');
                }
            }

            return httpPath;
        }

        private SwaggerOperationMethod GetMethod(MethodInfo method)
        {
            var methodName = method.Name;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpPostAttribute"))
                return SwaggerOperationMethod.Post;
            else if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpGetAttribute"))
                return SwaggerOperationMethod.Get;
            else if (methodName == "Get")
                return SwaggerOperationMethod.Get;
            else if (methodName == "Post")
                return SwaggerOperationMethod.Post;
            else if (methodName == "Put")
                return SwaggerOperationMethod.Put;
            else if (methodName == "Delete")
                return SwaggerOperationMethod.Delete;
            else
                return SwaggerOperationMethod.Post;
        }


        private void LoadParameters(AbpSwaggerOperation operation, IEnumerable<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            foreach (var parameter in parameters)
            {
                var info = JsonObjectTypeDescription.FromType(parameter.ParameterType);

                dynamic fromBodyAttribute = parameter.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

                dynamic fromUriAttribute = parameter.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "FromUriAttribute");

                // TODO: Add support for ModelBinder attribute

                var isComplexParameter = IsComplexType(info);
                if (isComplexParameter)
                {
                    if (fromUriAttribute != null)
                        AddPrimitiveParameter(operation, schemaResolver, parameter);
                    else
                        AddBodyParameter(operation, schemaResolver, parameter);
                }
                else
                {
                    if (fromBodyAttribute != null)
                        AddBodyParameter(operation, schemaResolver, parameter);
                    else
                        AddPrimitiveParameter(operation, schemaResolver, parameter);
                }
            }

            if (operation.Parameters.Count(p => p.Kind == AbpSwaggerParameterKind.Body) > 1)
                throw new InvalidOperationException("The operation '" + operation.OperationId + "' has more than one body parameter.");
        }

        private void AddBodyParameter(AbpSwaggerOperation operation, ISchemaResolver schemaResolver,
           ParameterInfo parameter)
        {
            var operationParameter = CreateBodyParameter(parameter, schemaResolver);
            operation.Parameters.Add(operationParameter);
        }

        private void AddPrimitiveParameter(AbpSwaggerOperation operation, ISchemaResolver schemaResolver,
            ParameterInfo parameter)
        {
            var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
            operationParameter.Kind = AbpSwaggerParameterKind.Query;
            operation.Parameters.Add(operationParameter);
        }

        private AbpSwaggerParameter CreateBodyParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            dynamic attr = parameter.GetCustomAttributes().SingleOrDefault(x => x.GetType().Name == "DescriptionAttribute");

            var operationParameter = new AbpSwaggerParameter
            {
                Schema = CreateAndAddSchema<AbpSwaggerParameter>(parameter.ParameterType, schemaResolver),
                //modify : modify by carl
                //Name = "request";
                Name = parameter.Name,
                Description = attr != null ? attr.Description : "",
                Kind = AbpSwaggerParameterKind.Body
            };

            return operationParameter;
        }

        private void LoadReturnType(AbpSwaggerOperation operation, MethodInfo method, ISchemaResolver schemaResolver)
        {
            if (method.ReturnType.FullName != "System.Void" && method.ReturnType.FullName != "System.Threading.Tasks.Task")
            {
                operation.Responses["200"] = new SwaggerResponse
                {
                    Description = "OK",
                    Schema = CreateAndAddSchema<JsonSchema4>(method.ReturnType, schemaResolver)
                };
            }
            else
                operation.Responses["200"] = new SwaggerResponse();
        }

        private TSchemaType CreateAndAddSchema<TSchemaType>(Type type, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            if (type.Name == "Task`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "JsonResult`1")
                type = type.GenericTypeArguments[0];

            var info = JsonObjectTypeDescription.FromType(type);
            if (info.Type.HasFlag(JsonObjectType.Object))
            {
                if (type == typeof(object))
                {
                    return new TSchemaType
                    {
                        Type = JsonObjectType.Object,
                        AllowAdditionalProperties = false
                    };
                }

                if (!schemaResolver.HasSchema(type, false))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
                    schemaGenerator.Generate<JsonSchema4>(type, schemaResolver);
                }

                return new TSchemaType
                {
                    Type = JsonObjectType.Object,
                    SchemaReference = schemaResolver.GetSchema(type, false)
                };
            }

            if (info.Type.HasFlag(JsonObjectType.Array))
            {
                var itemType = type.GenericTypeArguments.Length == 0 ? type.GetElementType() : type.GenericTypeArguments[0];
                return new TSchemaType
                {
                    Type = JsonObjectType.Array,
                    Item = CreateAndAddSchema<JsonSchema4>(itemType, schemaResolver)
                };
            }

            var generator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
            return generator.Generate<TSchemaType>(type, schemaResolver);
        }

        private AbpSwaggerParameter CreatePrimitiveParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var attrs = parameter.GetCustomAttributes();

            var parameterGenerator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
            var info = JsonObjectTypeDescription.FromType(parameter.ParameterType);
            var isComplexParameter = IsComplexType(info);
            var parameterType = isComplexParameter ? typeof(string) : parameter.ParameterType; // complex types must be treated as string

            var segmentParameter = parameterGenerator.Generate<AbpSwaggerParameter>(parameterType, schemaResolver);
            segmentParameter.Name = parameter.Name;

            dynamic attr = attrs.SingleOrDefault(x => x.GetType().Name == "DescriptionAttribute");
            segmentParameter.Description = attr != null ? attr.Description : "";
            segmentParameter.IsRequired = attrs.Any(x => x.GetType().Name == "RequiredAttribute");

            return segmentParameter;
        }

        private bool IsComplexType(JsonObjectTypeDescription info)
        {
            // TODO: Move to JsonObjectTypeDescription class in NJsonSchema
            return info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array);
        }
    }
}
