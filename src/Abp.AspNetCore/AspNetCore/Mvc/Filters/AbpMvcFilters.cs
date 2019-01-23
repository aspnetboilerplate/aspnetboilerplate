using System;
using System.Collections.Generic;
using System.Text;
using Abp.AspNetCore.Mvc.Auditing;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.AspNetCore.Mvc.Results;
using Abp.AspNetCore.Mvc.Uow;
using Abp.AspNetCore.Mvc.Validation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Filters
{
    public class AbpMvcFilters
    {
        public static void Register(FilterCollection filterCollection)
        {
            foreach (var filter in Filters)
            {
                filterCollection.AddService(filter);
            }
        }

        public static IList<Type> Filters = new List<Type>()
        {
            typeof(AbpAuthorizationFilter),
            typeof(AbpAuditActionFilter),
            typeof(AbpValidationActionFilter),
            typeof(AbpUowActionFilter),
            typeof(AbpExceptionFilter),
            typeof(AbpResultFilter)
        };

    }
}
