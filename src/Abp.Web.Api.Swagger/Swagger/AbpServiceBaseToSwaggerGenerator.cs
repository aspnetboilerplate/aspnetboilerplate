using NJsonSchema;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Web.Api.Swagger.Swagger;

namespace Abp.Web.Api.Swagger
{
    public class AbpServiceBaseToSwaggerGenerator
    {
        private SwaggerService _service;
        private readonly string _defaultRouteTemplate;
        private Type _serviceType;
        private JsonSchema4 _exceptionType;

        /// <summary>Initializes a new instance of the <see cref="AbpServiceBaseToSwaggerGenerator"/> class.</summary>
        /// <param name="defaultRouteTemplate">The default route template.</param>
        public AbpServiceBaseToSwaggerGenerator(string defaultRouteTemplate)
        {
            _defaultRouteTemplate = defaultRouteTemplate;
        }

      
        public SwaggerService Generate(Type type, InterfaceMapping map, string excludedMethodName = "Swagger",string controllernameused=null)
        {
            _service = new SwaggerService();
            _serviceType = type;

            _exceptionType = new JsonSchema4();
            _exceptionType.TypeName = "SwaggerException";
            _exceptionType.Properties.Add("ExceptionType", new JsonProperty { Type = JsonObjectType.String });
            _exceptionType.Properties.Add("Message", new JsonProperty { Type = JsonObjectType.String });
            _exceptionType.Properties.Add("StackTrace", new JsonProperty { Type = JsonObjectType.String });

            _service.Definitions[_exceptionType.TypeName] = _exceptionType;

            var schemaResolver = new SchemaResolver();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
         
            foreach (var method in methods.Where(m => m.Name != excludedMethodName))
            {
                if (Array.IndexOf(map.TargetMethods, method) == -1)
                {
                    continue;
                }
               
                var parameters = method.GetParameters().ToList();
                var methodName = method.Name;

                var operation = new SwaggerOperation();
                operation.OperationId = methodName;

                var httpPath = GetHttpPath(operation, method, parameters, schemaResolver, controllernameused);

                LoadParameters(operation, parameters, schemaResolver);
                LoadReturnType(operation, method, schemaResolver);
                LoadMetaData(operation, method);

                var httpMethod = GetMethod(method);

                if (!_service.Paths.ContainsKey(httpPath))
                {
                    var path = new SwaggerOperations();
                    _service.Paths[httpPath] = path;
                }

                _service.Paths[httpPath][httpMethod] = operation;
            }

            foreach (var schema in schemaResolver.Schemes)
                _service.Definitions[schema.TypeName] = schema;

            _service.GenerateOperationIds();
            return _service;
        }

        private void LoadMetaData(SwaggerOperation operation, MethodInfo method)
        {
            dynamic descriptionAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");

            if (descriptionAttribute != null)
                operation.Description = descriptionAttribute.Description;
        }

        private string GetHttpPath(SwaggerOperation operation, MethodInfo method, List<ParameterInfo> parameters, ISchemaResolver schemaResolver,string controllernameused)
        {
            var httpPath = string.Empty;

            dynamic routeAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "RouteAttribute");

            if (routeAttribute != null)
            {
                dynamic routePrefixAttribute = _serviceType.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "RoutePrefixAttribute");

                if (routePrefixAttribute != null)
                    httpPath = routePrefixAttribute.Prefix + "/" + routeAttribute.Template;
                else
                    httpPath = routeAttribute.Template;
            }
            else
            {
                //Abp 约定
                var controllername = _serviceType.Name.Replace("AppService", string.Empty);
                controllername = controllername.Replace("Service", string.Empty);
                if (!string.IsNullOrEmpty(controllernameused))
                {
                    controllername = controllernameused;
                }

                httpPath = _defaultRouteTemplate
                    .Replace("{controller}",controllername )
                    .Replace("{action}", method.Name);
            }

            foreach (var match in Regex.Matches(httpPath, "\\{(.*?)\\}").OfType<Match>())
            {
                var parameter = parameters.SingleOrDefault(p => p.Name == match.Groups[1].Value);
                if (parameter != null)
                {
                    var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
                    operationParameter.Kind = SwaggerParameterKind.path;

                    //var queryParameter = operation.Parameters.SingleOrDefault(p => p.Name == operationParameter.Name);
                    //if (queryParameter != null)
                    //    operation.Parameters.Remove(queryParameter);

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
                return SwaggerOperationMethod.post;
            else if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpGetAttribute"))
                return SwaggerOperationMethod.get;
            else if (methodName == "Get")
                return SwaggerOperationMethod.get;
            else if (methodName == "Post")
                return SwaggerOperationMethod.post;
            else if (methodName == "Put")
                return SwaggerOperationMethod.put;
            else if (methodName == "Delete")
                return SwaggerOperationMethod.delete;
            else
                return SwaggerOperationMethod.post;
        }

        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        private void LoadParameters(SwaggerOperation operation, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            foreach (var parameter in parameters)
            {
                dynamic fromBodyAttribute = parameter.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

                if (fromBodyAttribute != null)
                {
                    var operationParameter = CreateBodyParameter(parameter, schemaResolver);
                    operation.Parameters.Add(operationParameter);
                }
                else
                {
                    var info = JsonObjectTypeDescription.FromType(parameter.ParameterType);
                    if (info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array))
                    {
                        if (operation.Parameters.Any(p => p.Kind == SwaggerParameterKind.body))
                            throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array. ");

                        var operationParameter = CreateBodyParameter(parameter, schemaResolver);
                        operation.Parameters.Add(operationParameter);
                    }
                    else
                    {
                        var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
                        operationParameter.Kind = SwaggerParameterKind.query;

                        operation.Parameters.Add(operationParameter);
                    }
                }
            }
        }

        private SwaggerParameter CreateBodyParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var operationParameter = new SwaggerParameter();
            operationParameter.Schema = CreateAndAddSchema<SwaggerParameter>(parameter.ParameterType, schemaResolver);
            operationParameter.Name = "request";
            operationParameter.Kind = SwaggerParameterKind.body;
            return operationParameter;
        }

        private void LoadReturnType(SwaggerOperation operation, MethodInfo method, ISchemaResolver schemaResolver)
        {
            if (method.ReturnType.FullName != "System.Void" && method.ReturnType.FullName != "System.Threading.Tasks.Task")
            {
                operation.Responses["200"] = new SwaggerResponse
                {
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
                if (!schemaResolver.HasSchema(type))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(_service);
                    schemaGenerator.Generate<JsonSchema4>(type, schemaResolver);
                }

                //load 
                //目前先屏蔽处理某些特定类型的异常
                //Dictionary<string,string>
                JsonSchema4 t = null;
                try
                {
                    t = schemaResolver.GetSchema(type);
                }
                catch (Exception)
                {

                }
                return new TSchemaType
                {
                    Type = JsonObjectType.Object,
                    SchemaReference = t
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

            var generator = new RootTypeJsonSchemaGenerator(_service);
            return generator.Generate<TSchemaType>(type, schemaResolver);
        }

        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        private SwaggerParameter CreatePrimitiveParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var parameterType = parameter.ParameterType;

            //处理Guid
            if (parameterType == typeof(Guid))
            {
                parameterType = typeof(string);
            }

            var info = JsonObjectTypeDescription.FromType(parameterType);
            if (info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array))
                throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array.");

            var parameterGenerator = new RootTypeJsonSchemaGenerator(_service);

            var segmentParameter = parameterGenerator.Generate<SwaggerParameter>(parameter.ParameterType, schemaResolver);
            segmentParameter.Name = parameter.Name;
            return segmentParameter;
        }
    }
}
