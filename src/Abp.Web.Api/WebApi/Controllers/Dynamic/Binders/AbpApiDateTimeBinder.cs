using System;
using System.Globalization;
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

            bindingContext.Model = Clock.Normalize(date.Value);
            return true;
        }
    }
}
