using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Auditing;
using Abp.Dependency;
using Abp.WebApi.Validation;

namespace Abp.WebApi.Auditing
{
    public class AbpApiAuditFilter : IActionFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IAuditingHelper _auditingHelper;

        public AbpApiAuditFilter(IAuditingHelper auditingHelper)
        {
            _auditingHelper = auditingHelper;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var method = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (method == null || !ShouldSaveAudit(actionContext))
            {
                return await continuation();
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(
                method,
                actionContext.ActionArguments
            );

            var stopwatch = Stopwatch.StartNew();

            try
            {
                return await continuation();
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