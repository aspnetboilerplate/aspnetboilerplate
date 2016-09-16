using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Aspects;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Auditing
{
    public class AbpAuditActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IAbpAspNetCoreConfiguration _configuration;
        private readonly IAuditingHelper _auditingHelper;

        public AbpAuditActionFilter(IAbpAspNetCoreConfiguration configuration, IAuditingHelper auditingHelper)
        {
            _configuration = configuration;
            _auditingHelper = auditingHelper;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldSaveAudit(context))
            {
                await next();
                return;
            }

            using (AbpCrossCuttingConcerns.Applying(context.Controller, AbpCrossCuttingConcerns.Auditing))
            {
                var auditInfo = _auditingHelper.CreateAuditInfo(
                    context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo,
                    context.ActionArguments
                );

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    var result = await next();
                    if (result.Exception != null && !result.ExceptionHandled)
                    {
                        auditInfo.Exception = result.Exception;
                    }
                }
                catch (Exception ex)
                {
                    auditInfo.Exception = ex;
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                    await _auditingHelper.SaveAsync(auditInfo);
                }
            }
        }

        private bool ShouldSaveAudit(ActionExecutingContext actionContext)
        {
            return _configuration.IsAuditingEnabled &&
                   _auditingHelper.ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true);
        }
    }
}
