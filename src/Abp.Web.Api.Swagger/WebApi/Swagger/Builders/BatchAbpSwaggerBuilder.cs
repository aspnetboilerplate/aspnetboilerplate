using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Abp.Dependency;
using Abp.Extensions;
using Abp.WebApi.Swagger.Configuration;
using Newtonsoft.Json.Serialization;
using NJsonSchema;

namespace Abp.WebApi.Swagger.Builders
{
    internal class BatchAbpSwaggerBuilder<T> : IBatchAbpSwaggerBuilder<T>
    {
        private const string UrlTemplate = "api/services/{servicePrefix}/{controller}/{action}/{id}";

        private Func<Type, bool> _typePredicate;

        private readonly string _servicePrefix;
        private readonly Assembly _assembly;
        private Func<Type, string> _serviceNameSelector;
        private string _relativePath = "WebApiDoc";

        public BatchAbpSwaggerBuilder(Assembly assembly, string servicePrefix)
        {
            _assembly = assembly;
            _servicePrefix = servicePrefix;
        }

        public IBatchAbpSwaggerBuilder<T> RelativePath(string relativePath)
        {
            _relativePath = relativePath;
            return this;
        }

        public IBatchAbpSwaggerBuilder<T> Where(Func<Type, bool> predicate)
        {
            _typePredicate = predicate;
            return this;
        }

        public IBatchAbpSwaggerBuilder<T> WithServiceName(Func<Type, string> serviceNameSelector)
        {
            _serviceNameSelector = serviceNameSelector;
            return this;
        }

        public IAbpSwaggerEnabledConfiguration Build(string moduleName)
        {
            var model = AbpSwaggerHelper.AbpSwaggerModel;

            model.Modules.Add(moduleName);

            var types =
                from
                    type in _assembly.GetTypes()
                where
                    type.IsPublic && type.IsInterface && typeof(T).IsAssignableFrom(type) && IocManager.Instance.IsRegistered(type)
                select
                    type;

            if (_typePredicate != null)
            {
                types = types.Where(t => _typePredicate(t));
            }

            var urlTemplate = UrlTemplate.Replace("{servicePrefix}", _servicePrefix);
            var rootPath = AbpSwaggerHelper.GetApplicationPath() + "/" + _relativePath;
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var swaggerGenerator = new AbpSwaggerGenerator(urlTemplate, new JsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = true
            });

            var serviceNames = new List<string>();

            var textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

            foreach (var type in types)
            {
                var serviceName = _serviceNameSelector != null
                   ? _serviceNameSelector(type)
                   : GetConventionalServiceName(type);

                var service = swaggerGenerator.GenerateForAbpAppService(serviceName, type);

                if (service == null)
                    continue;

                var strJson = service.ToJson();
              
                var docPath = rootPath + "/" + moduleName;
                if (!Directory.Exists(docPath))
                {
                    Directory.CreateDirectory(docPath);
                }

                var docFullName = docPath + "/" + serviceName + ".js";
                if (File.Exists(docFullName))
                {
                    File.Delete(docFullName);
                }

                using (var writer = File.CreateText(docFullName))
                {
                    writer.Write(strJson);
                    writer.Flush();
                    writer.Close();
                }

                serviceNames.Add(textInfo.ToTitleCase(serviceName));
            }

            model.Services.Add(moduleName, serviceNames);

            var configuration = IocManager.Instance.Resolve<IAbpSwaggerModuleConfiguration>();

            var abpSwagger = new AbpSwaggerEnabledConfiguration(
                configuration.HttpConfiguration,
                AbpSwaggerHelper.DefaultRootUrlResolver,
                configuration.AbpSwaggerUiConfigure
                );

            return abpSwagger;
        }

        public static string GetConventionalServiceName(Type type)
        {
            var typeName = type.Name;

            if (typeName.EndsWith("ApplicationService"))
            {
                typeName = typeName.Substring(0, typeName.Length - "ApplicationService".Length);
            }
            else if (typeName.EndsWith("AppService"))
            {
                typeName = typeName.Substring(0, typeName.Length - "AppService".Length);
            }
            else if (typeName.EndsWith("Service"))
            {
                typeName = typeName.Substring(0, typeName.Length - "Service".Length);
            }

            if (typeName.Length > 1 && typeName.StartsWith("I") && char.IsUpper(typeName, 1))
            {
                typeName = typeName.Substring(1);
            }

            return typeName.ToCamelCase();
        }
    }
}
