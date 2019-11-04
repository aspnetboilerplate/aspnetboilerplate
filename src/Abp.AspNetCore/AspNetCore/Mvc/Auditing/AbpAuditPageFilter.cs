using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Aspects;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Auditing
{
    public class AbpAuditPageFilter : IAsyncPageFilter, ITransientDependency
    {
        private readonly IAbpAspNetCoreConfiguration _configuration;
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditingConfiguration _auditingConfiguration;
        private readonly IAuditSerializer _auditSerializer;

        public AbpAuditPageFilter(IAbpAspNetCoreConfiguration configuration, 
            IAuditingHelper auditingHelper,
            IAuditingConfiguration auditingConfiguration, 
            IAuditSerializer auditSerializer)
        {
            _configuration = configuration;
            _auditingHelper = auditingHelper;
            _auditingConfiguration = auditingConfiguration;
            _auditSerializer = auditSerializer;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod == null || !ShouldSaveAudit(context))
            {
                await next();
                return;
            }

            using (AbpCrossCuttingConcerns.Applying(context.HandlerInstance, AbpCrossCuttingConcerns.Auditing))
            {
                var auditInfo = _auditingHelper.CreateAuditInfo(
                    context.HandlerInstance.GetType(),
                    context.HandlerMethod.MethodInfo,
                    context.GetBoundPropertiesAsDictionary()
                );

                var stopwatch = Stopwatch.StartNew();

                PageHandlerExecutedContext result = null;
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
                                if (objectResult.Value is AjaxResponse ajaxObjectResponse)
                                {
                                    auditInfo.ReturnValue = _auditSerializer.Serialize(ajaxObjectResponse.Result);
                                }
                                else
                                {
                                    auditInfo.ReturnValue = _auditSerializer.Serialize(objectResult.Value);
                                }
                                break;

                            case JsonResult jsonResult:
                                if (jsonResult.Value is AjaxResponse ajaxJsonResponse)
                                {
                                    auditInfo.ReturnValue = _auditSerializer.Serialize(ajaxJsonResponse.Result);
                                }
                                else
                                {
                                    auditInfo.ReturnValue = _auditSerializer.Serialize(jsonResult.Value);
                                }
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

        private bool ShouldSaveAudit(PageHandlerExecutingContext actionContext)
        {
            return _configuration.IsAuditingEnabled &&
                   _auditingHelper.ShouldSaveAudit(actionContext.HandlerMethod.MethodInfo, true);
        }
    }
}
