using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Web.Models;
using Abp.WebApi.Validation;

namespace Abp.WebApi.Auditing
{
    public class AbpApiAuditFilter : IActionFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditingConfiguration _auditingConfiguration;
        private readonly IAuditSerializer _auditSerializer;

        public AbpApiAuditFilter(IAuditingHelper auditingHelper, 
            IAuditingConfiguration auditingConfiguration,
            IAuditSerializer auditSerializer)
        {
            _auditingHelper = auditingHelper;
            _auditingConfiguration = auditingConfiguration;
            _auditSerializer = auditSerializer;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var method = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (method == null || !ShouldSaveAudit(actionContext))
            {
                return await continuation();
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(
                actionContext.ActionDescriptor.ControllerDescriptor.ControllerType,
                method,
                actionContext.ActionArguments
            );

            var stopwatch = Stopwatch.StartNew();

            HttpResponseMessage response = null;
            try
            {
                response = await continuation();
                return response;
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

                if (_auditingConfiguration.SaveReturnValues && response != null)
                {
                    if (response.TryGetContentValue(out object resultObject) )
                    {
                        auditInfo.ReturnValue = _auditSerializer.Serialize(resultObject);
                    }
                }

                await _auditingHelper.SaveAsync(auditInfo);
            }
        }

        private bool ShouldSaveAudit(HttpActionContext context)
        {
            if (context.ActionDescriptor.IsDynamicAbpAction())
            {
                return false;
            }

            return _auditingHelper.ShouldSaveAudit(context.ActionDescriptor.GetMethodInfoOrNull(), true);
        }
    }
}