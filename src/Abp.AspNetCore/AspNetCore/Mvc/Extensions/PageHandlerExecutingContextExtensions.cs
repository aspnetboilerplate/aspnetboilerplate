using System.Collections.Generic;
using System.Linq;
using Abp.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Extensions
{
    public static class PageHandlerExecutingContextExtensions
    {
        public static Dictionary<string, object> GetBoundPropertiesAsDictionary(this PageHandlerExecutingContext context)
        {
            if (!context.ActionDescriptor.BoundProperties.Any())
            {
                return new Dictionary<string, object>();
            }

            var result = new Dictionary<string, object>();

            foreach (var boundProperty in context.ActionDescriptor.BoundProperties)
            {
                var value = ReflectionHelper.GetValueByPath(
                    context.HandlerInstance,
                    context.HandlerInstance.GetType(),
                    boundProperty.Name);

                result.Add(boundProperty.Name, value);
            }

            return result;
        }
    }
}
