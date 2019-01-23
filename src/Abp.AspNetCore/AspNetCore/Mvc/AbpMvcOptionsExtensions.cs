using Abp.AspNetCore.Mvc.Auditing;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetCore.Mvc.Conventions;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.AspNetCore.Mvc.Filters;
using Abp.AspNetCore.Mvc.ModelBinding;
using Abp.AspNetCore.Mvc.Results;
using Abp.AspNetCore.Mvc.Uow;
using Abp.AspNetCore.Mvc.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.AspNetCore.Mvc
{
    internal static class AbpMvcOptionsExtensions
    {
        public static void AddAbp(this MvcOptions options, IServiceCollection services)
        {
            AddConventions(options, services);
            AbpMvcFilters.Register(options.Filters);
            AddModelBinders(options);
        }

        private static void AddConventions(MvcOptions options, IServiceCollection services)
        {
            options.Conventions.Add(new AbpAppServiceConvention(services));
        }

        private static void AddModelBinders(MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new AbpDateTimeModelBinderProvider());
        }
    }
}