using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Aspects;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Auditing
{
    public class AbpAuditActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IAbpAspNetCoreConfiguration _configuration;
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditingConfiguration _auditingConfiguration;
        private readonly IAuditSerializer _auditSerializer;

        public AbpAuditActionFilter(IAbpAspNetCoreConfiguration configuration,
            IAuditingHelper auditingHelper,
            IAuditingConfiguration auditingConfiguration,
            IAuditSerializer auditSerializer)
        {
            _configuration = configuration;
            _auditingHelper = auditingHelper;
            _auditingConfiguration = auditingConfiguration;
            _auditSerializer = auditSerializer;
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
                    context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType(),
                    context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo,
                    context.ActionArguments
                );

                var stopwatch = Stopwatch.StartNew();

                ActionExecutedContext result = null;
                try
                {
                    result = await next();
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

                    if (_auditingConfiguration.SaveReturnValues && result != null)
                    {
                        switch (result.Result)
                        {
                            case ObjectResult objectResult:
                                auditInfo.ReturnValue = _auditSerializer.Serialize(objectResult.Value);
                                break;

                            case JsonResult jsonResult:
                                auditInfo.ReturnValue = _auditSerializer.Serialize(jsonResult.Value);
                                break;

                            case ContentResult contentResult:
                                auditInfo.ReturnValue = contentResult.Content;
                                break;
                        }
                    }

                    await _auditingHelper.SaveAsync(auditInfo);
                }
            }
        }

        private bool ShouldSaveAudit(ActionExecutingContext actionContext)
        {
            return _configuration.IsAuditingEnabled &&
                   actionContext.ActionDescriptor.IsControllerAction() &&
                   _auditingHelper.ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true);
        }
    }
}
