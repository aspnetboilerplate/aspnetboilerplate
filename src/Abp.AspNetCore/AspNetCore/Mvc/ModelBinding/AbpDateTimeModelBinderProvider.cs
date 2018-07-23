using System;
using Abp.Timing;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Abp.AspNetCore.Mvc.ModelBinding
{
    public class AbpDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(DateTime) &&
                context.Metadata.ModelType != typeof(DateTime?))
            {
                return null;
            }

            if (context.Metadata.ContainerType == null)
            {
                return null;
            }

            var dateNormalizationDisabledForClass = context.Metadata.ContainerType.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true);
            var dateNormalizationDisabledForProperty = context.Metadata.ContainerType
                                                                        .GetProperty(context.Metadata.PropertyName)
                                                                        .IsDefined(typeof(DisableDateTimeNormalizationAttribute), true);

            if (!dateNormalizationDisabledForClass && !dateNormalizationDisabledForProperty)
            {
                return new AbpDateTimeModelBinder(context.Metadata.ModelType);
            }

            return null;
        }
    }
}