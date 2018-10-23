using System;
using System.Globalization;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Abp.Timing;

namespace Abp.WebApi.Controllers.Dynamic.Binders
{
    /// <summary>
    /// Binds datetime values from api requests to model
    /// </summary>
    public class AbpApiDateTimeBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var date = value?.ConvertTo(typeof(DateTime?), CultureInfo.CurrentCulture) as DateTime?;
            if (date == null)
            {
                return true;
            }

            if (bindingContext.ModelMetadata.ContainerType != null)
            {
                if (bindingContext.ModelMetadata.ContainerType.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true))
                {
                    bindingContext.Model = date.Value;
                    return true;
                }

                var property = bindingContext.ModelMetadata.ContainerType.GetProperty(bindingContext.ModelName);

                if (property != null && property.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true))
                {
                    bindingContext.Model = date.Value;
                    return true;
                }
            }

            if (actionContext.ActionDescriptor == null)
            {
                bindingContext.Model = Clock.Normalize(date.Value);
                return true;
            }

            var parameter = actionContext.ActionDescriptor.GetParameters().FirstOrDefault(p => p.ParameterName == bindingContext.ModelName);
            if (parameter != null && parameter.GetCustomAttributes<DisableDateTimeNormalizationAttribute>().Count > 0)
            {
                bindingContext.Model = date.Value;
                return true;
            }

            bindingContext.Model = Clock.Normalize(date.Value);
            return true;
        }
    }
}
