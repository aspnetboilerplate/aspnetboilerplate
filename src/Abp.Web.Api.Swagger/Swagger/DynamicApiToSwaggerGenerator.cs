using NJsonSchema;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Abp.WebApi.Controllers.Dynamic;
using System.Xml.XPath;
using System.IO;

namespace Abp.Web.Api.Swagger
{
    public class DynamicApiToSwaggerGenerator
    {
     
        private readonly string _urlprix;
        public JsonSchemaGeneratorSettings JsonSchemaGeneratorSettings { get; set; }
    
        public DynamicApiToSwaggerGenerator(string urlprix, JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings)
        {
            _urlprix = urlprix;
            JsonSchemaGeneratorSettings = jsonSchemaGeneratorSettings;
        }

      
        public SwaggerService Generate(SwaggerService root,DynamicApiControllerInfo info)
        {
            Dictionary<string, Type> find = new Dictionary<string, Type>();
            //tag
            root.Tags.Add(new SwaggerTag() { Name = info.ServiceName } );      
         
          
            var schemaResolver = new SchemaResolver();          
         
            foreach (var action in info.Actions)
            {

                var method = action.Value.Method;
                var parameters = method.GetParameters().ToList();
                var methodName = method.Name;

                var operation = new SwaggerOperation();
                operation.OperationId = methodName;              
                operation.Summary = action.Key;
                operation.Tags = new List<string>() { info.ServiceName };
            
                var httpPath = GetHttpPath(root,operation, method, parameters,info,action.Value, schemaResolver);

                LoadParameters(root,operation, parameters, schemaResolver,find);
                LoadReturnType(root,operation, method, schemaResolver,find);
                LoadMetaData(operation, method);

                var httpMethod = GetMethod(action.Value.Verb);

                if (!root.Paths.ContainsKey(httpPath))
                {
                    var path = new SwaggerOperations();
                    root.Paths[httpPath] = path;
                }

                root.Paths[httpPath][httpMethod] = operation;

                //xml comment
                XmlCommentHelper.ApplyXMLComment(info,action.Value,operation);
            }

            foreach (var schema in schemaResolver.Schemes)
            {
                root.Definitions[schema.TypeName] = schema;
                //add model xml comment
                XmlCommentHelper.ApplyXmlTypeComments(schema,find);
            }
            root.GenerateOperationIds();
            return root;
        }


        private void LoadMetaData(SwaggerOperation operation, MethodInfo method)
        {
            dynamic descriptionAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");

            if (descriptionAttribute != null)
                operation.Description = descriptionAttribute.Description;
        }

        private string GetHttpPath(SwaggerService root,SwaggerOperation operation, MethodInfo method, List<ParameterInfo> parameters, DynamicApiControllerInfo info,DynamicApiActionInfo action, ISchemaResolver schemaResolver)
        {
            var httpPath = string.Empty;
            //start with dash
            httpPath = "/"+_urlprix+"/"+ info.ServiceName + "/" + action.ActionName;                 
            

            foreach (var match in Regex.Matches(httpPath, "\\{(.*?)\\}").OfType<Match>())
            {
                var parameter = parameters.SingleOrDefault(p => p.Name == match.Groups[1].Value);
                if (parameter != null)
                {
                    var operationParameter = CreatePrimitiveParameter(root,parameter, schemaResolver);
                    operationParameter.Kind = SwaggerParameterKind.Path;

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

        private SwaggerOperationMethod GetMethod(Abp.Web.HttpVerb verb)
        {            
            if (verb== HttpVerb.Post)
                return SwaggerOperationMethod.Post;
            else if (verb == HttpVerb.Get)
                return SwaggerOperationMethod.Get;
            else if (verb == HttpVerb.Delete)
                return SwaggerOperationMethod.Delete;
            else if (verb == HttpVerb.Put)
                return SwaggerOperationMethod.Put;           
            else
                return SwaggerOperationMethod.Post;
        }

        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        private void LoadParameters(SwaggerService root,SwaggerOperation operation, List<ParameterInfo> parameters, ISchemaResolver schemaResolver, Dictionary<string, Type> find)
        {
            foreach (var parameter in parameters)
            {
                dynamic fromBodyAttribute = parameter.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

                if (fromBodyAttribute != null)
                {
                    var operationParameter = CreateBodyParameter(root,parameter, schemaResolver,find);
                    operation.Parameters.Add(operationParameter);
                }
                else
                {
                    var info = JsonObjectTypeDescription.FromType(parameter.ParameterType);
                    if (info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array))
                    {
                        if (operation.Parameters.Any(p => p.Kind == SwaggerParameterKind.Body))
                            throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array. ");

                        var operationParameter = CreateBodyParameter(root,parameter, schemaResolver,find);
                        operation.Parameters.Add(operationParameter);
                    }
                    else
                    {
                        var operationParameter = CreatePrimitiveParameter(root,parameter, schemaResolver);
                        operationParameter.Kind = SwaggerParameterKind.Query;

                        operation.Parameters.Add(operationParameter);
                    }
                }
            }
        }

        private SwaggerParameter CreateBodyParameter(SwaggerService root,ParameterInfo parameter, ISchemaResolver schemaResolver, Dictionary<string, Type> find)
        {
            var operationParameter = new SwaggerParameter();
            operationParameter.Schema = CreateAndAddSchema<SwaggerParameter>(root,parameter.ParameterType, schemaResolver,find);
            if (operationParameter.Schema.SchemaReference!=null&&!find.Keys.Contains(operationParameter.Schema.SchemaReference.TypeName))
            {
                find.Add(operationParameter.Schema.SchemaReference.TypeName, parameter.ParameterType);
            }
            operationParameter.Name = "request";
            operationParameter.Kind = SwaggerParameterKind.Body;
            return operationParameter;
        }

        private void LoadReturnType(SwaggerService root,SwaggerOperation operation, MethodInfo method, ISchemaResolver schemaResolver, Dictionary<string, Type> find)
        {
            if (method.ReturnType.FullName != "System.Void" && method.ReturnType.FullName != "System.Threading.Tasks.Task")
            {
                var schema = CreateAndAddSchema<JsonSchema4>(root, method.ReturnType, schemaResolver,find);
                operation.Responses["200"] = new SwaggerResponse
                {
                    Schema =schema
                };
                if (schema.SchemaReference!=null&&!find.Keys.Contains(schema.SchemaReference.TypeName))
                {
                    find.Add(schema.SchemaReference.TypeName, method.ReturnType);
                }

            }
            else
                operation.Responses["200"] = new SwaggerResponse();
        }

        private TSchemaType CreateAndAddSchema<TSchemaType>(SwaggerService root,Type type, ISchemaResolver schemaResolver, Dictionary<string, Type> find)
            where TSchemaType : JsonSchema4, new()
        {
            if (type.Name == "Task`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "JsonResult`1")
                type = type.GenericTypeArguments[0];

            var info = JsonObjectTypeDescription.FromType(type);

            if (info.Type.HasFlag(JsonObjectType.Object))
            {
                if (!schemaResolver.HasSchema(type,false))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(root, JsonSchemaGeneratorSettings);
                    schemaGenerator.Generate<JsonSchema4>(type, schemaResolver);
                }

                //load 
                //目前先屏蔽处理某些特定类型的异常
                //Dictionary<string,string>
                JsonSchema4 t = null;
                try
                {
                    t = schemaResolver.GetSchema(type,false);
                }
                catch (Exception)
                {

                }
                return new TSchemaType
                {
                    Type = JsonObjectType.Object,
                    //TypeName=t.TypeName,
                    SchemaReference = t
                };
            }

            if (info.Type.HasFlag(JsonObjectType.Array))
            {
                var itemType = type.GenericTypeArguments.Length == 0 ? type.GetElementType() : type.GenericTypeArguments[0];
                var schema = CreateAndAddSchema<JsonSchema4>(root, itemType, schemaResolver, find);
                if (schema.SchemaReference!=null&&!find.Keys.Contains(schema.SchemaReference.TypeName))
                {
                    find.Add(schema.SchemaReference.TypeName, itemType);
                }
                return new TSchemaType
                {
                    Type = JsonObjectType.Array,
                    Item =schema
                };

                
            }

            var generator = new RootTypeJsonSchemaGenerator(root, JsonSchemaGeneratorSettings);
            return generator.Generate<TSchemaType>(type, schemaResolver);
        }

        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        private SwaggerParameter CreatePrimitiveParameter(SwaggerService root,ParameterInfo parameter, ISchemaResolver schemaResolver)
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

            var parameterGenerator = new RootTypeJsonSchemaGenerator(root, JsonSchemaGeneratorSettings);

            var segmentParameter = parameterGenerator.Generate<SwaggerParameter>(parameter.ParameterType, schemaResolver);
            segmentParameter.Name = parameter.Name;
            return segmentParameter;
        }
    }
}
