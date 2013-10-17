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
        private readonly List<Type> _types = new List<Type>();

        private void FillAllTypes()
        {
            var controllers = DynamicApiControllerManager.GetAllServiceControllers();
            foreach (var controller in controllers)
            {
                foreach (var action in controller.Actions.Values)
                {
                    foreach (var parameterInfo in action.Method.GetParameters())
                    {
                        AddTypeRecursively(parameterInfo.ParameterType);
                    }

                    AddTypeRecursively(action.Method.ReturnType);
                }
            }
        }

        private void AddTypeRecursively(Type type)
        {
            if (!typeof(IDto).IsAssignableFrom(type))
            {
                return;
            }

            if (_types.Contains(type))
            {
                return;
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                AddTypeRecursively(propertyInfo.PropertyType);

                if (propertyInfo.PropertyType.IsGenericType)
                {
                    foreach (var genericArgs in propertyInfo.PropertyType.GenericTypeArguments)
                    {
                        AddTypeRecursively(genericArgs);
                    }
                }
            }

            _types.Add(type);
        }

        public string GenerateAll()
        {
            FillAllTypes();

            var script = new StringBuilder();

            script.AppendLine("define(['jquery', 'knockout'], function ($, ko) {");
            script.AppendLine();
            script.AppendLine("    var dtos = dtos || {};");

            foreach (var type in _types)
            {
                script.AppendLine();
                script.AppendLine("    dtos." + type.Name + " = function() {");
                foreach (var property in type.GetProperties())
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

        private bool IsArrayLikeType(Type type)
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
