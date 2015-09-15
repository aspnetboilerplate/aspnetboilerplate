using Abp.Web.Api.Swagger;
using NJsonSchema;
using NSwag;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Swagger
{
    public class AssemblyTypeToSwaggerGenerator
    {
        private readonly string _assemblyPath;

        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGenerator"/> class.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public AssemblyTypeToSwaggerGenerator(string assemblyPath)
        {
            _assemblyPath = assemblyPath;
        }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetAbpServiceBaseClasses()
        {
            var assembly = Assembly.LoadFrom(_assemblyPath);
            return assembly.ExportedTypes
                .Where(t => t.BaseType != null && t.BaseType.Name == "AppServiceBase")
                .Select(t => t.FullName)
                .ToArray();
        }

    


        public SwaggerService FromAbpApplicationMoudleAssembly( string controllerClassName, string urlTemplate)
        {
            var assembly = Assembly.LoadFrom(_assemblyPath);
            var type = assembly.GetType(controllerClassName);
            var interfacetype = type.GetInterface("I" + type.Name);
            var map = type.GetInterfaceMap(interfacetype);
            var generator = new AbpServiceBaseToSwaggerGenerator(urlTemplate);
            return generator.Generate(type,map);
            
        }

       

        private class NSwagServiceLoader : MarshalByRefObject
        {
          

            internal string FromAssemblyType(string assemblyPath, string className)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(className);

                var service = new SwaggerService();
                var schema = JsonSchema4.FromType(type);
                service.Definitions[type.Name] = schema;
                return service.ToJson();
            }

           

            internal string[] GetClasses(string assemblyPath)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.ExportedTypes
                    .Select(t => t.FullName)
                    .OrderBy(t => t)
                    .ToArray();
            }
        }
    }
}
