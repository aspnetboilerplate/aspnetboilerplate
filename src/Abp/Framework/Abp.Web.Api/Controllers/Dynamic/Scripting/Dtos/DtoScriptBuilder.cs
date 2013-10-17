using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Dtos
{
    public class DtoScriptBuilder
    {
        private class TypeInfo
        {
            public Type Type { get; private set; }

            public DynamicApiControllerInfo Controller { get; private set; }

            public TypeInfo(DynamicApiControllerInfo controller, Type type)
            {
                Controller = controller;
                Type = type;
            }
        }

        private readonly List<TypeInfo> _types = new List<TypeInfo>();

        private void FillAllTypes()
        {
            var controllers = DynamicApiControllerManager.GetAllServiceControllers();
            foreach (var controller in controllers)
            {
                foreach (var action in controller.Actions.Values)
                {
                    foreach (var parameterInfo in action.Method.GetParameters())
                    {
                        AddTypeRecursively(controller, parameterInfo.ParameterType);
                    }

                    AddTypeRecursively(controller, action.Method.ReturnType);
                }
            }
        }

        private void AddTypeRecursively(DynamicApiControllerInfo controller, Type type)
        {
            if (!typeof(IDto).IsAssignableFrom(type))
            {
                return;
            }

            if (_types.FirstOrDefault(t => t.Type == type) != null)
            {
                return;
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                AddTypeRecursively(controller, propertyInfo.PropertyType);

                if (propertyInfo.PropertyType.IsGenericType)
                {
                    foreach (var genericArgs in propertyInfo.PropertyType.GenericTypeArguments)
                    {
                        AddTypeRecursively(controller, genericArgs);
                    }
                }
            }

            _types.Add(new TypeInfo(controller, type));
        }

        public string GenerateAll()
        {
            FillAllTypes();

            var script = new StringBuilder();

            script.AppendLine("define(['jquery', 'knockout'], function ($, ko) {");
            script.AppendLine();
            script.AppendLine("    var dtos = dtos || {};");
            
            var areaNames = _types.Select(t => t.Controller.AreaName).Distinct().ToList();
            foreach (var areaName in areaNames)
            {
                if (string.IsNullOrWhiteSpace(areaName))
                {
                    continue;
                }

                script.AppendLine("    dtos." + areaName.ToCamelCase() + " = dtos." + areaName.ToCamelCase() + " || {};");
            }

            foreach (var areaName in areaNames)
            {
                var areaNameToGetController = areaName;
                var controllerNames = _types.Where(t => t.Controller.AreaName == areaNameToGetController).Select(t => t.Controller.Name).Distinct();
                foreach (var controllerName in controllerNames)
                {
                    if (string.IsNullOrWhiteSpace(areaName))
                    {
                        script.AppendLine("    dtos." + controllerName.ToCamelCase() + " = dtos." + controllerName.ToCamelCase() + " || {};");                        
                    }
                    else
                    {
                        script.AppendLine("    dtos." + areaName.ToCamelCase() + "." + controllerName.ToCamelCase() + " = dtos." + areaName.ToCamelCase() + "." + controllerName.ToCamelCase() + " || {};");                        
                    }
                }
            }
            
            foreach (var typeInfo in _types)
            {

                var typeWithNs = typeInfo.Controller.Name.ToCamelCase() + "." + typeInfo.Type.Name;
                if (!string.IsNullOrWhiteSpace(typeInfo.Controller.AreaName))
                {
                    typeWithNs = typeInfo.Controller.AreaName.ToCamelCase() + "." + typeWithNs;
                }

                script.AppendLine();
                script.AppendLine("    dtos." + typeWithNs + " = function() {");
                foreach (var property in typeInfo.Type.GetProperties())
                {
                    if (IsArrayLikeType(property.PropertyType))
                    {
                        script.AppendLine("        this." + property.Name.ToCamelCase() + " = ko.observableArray();");
                    }
                    else
                    {
                        script.AppendLine("        this." + property.Name.ToCamelCase() + " = ko.observable();");
                    }
                }

                script.AppendLine("    };");
            }

            script.AppendLine();
            script.AppendLine("    return dtos;");
            script.AppendLine();
            script.AppendLine("});");

            return script.ToString();
        }

        private static bool IsArrayLikeType(Type type)
        {
            if (type.IsArray) //TODO: Test!
            {
                return true;
            }

            if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)))
            {
                return true;
            }

            return false;
        }
    }
}
