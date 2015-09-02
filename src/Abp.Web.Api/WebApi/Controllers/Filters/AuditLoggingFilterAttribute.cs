using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Auditing;
using Abp.Dependency;
using Castle.Core.Logging;

namespace Abp.WebApi.Controllers.Filters
{
    //TODO: This will be implemented to save audit logs for regular web api methods.
    //public class AuditLoggingFilterAttribute : ActionFilterAttribute, ITransientDependency
    //{
    //    public ILogger Logger { get; set; }
        
    //    private readonly IAuditingConfiguration _auditingConfiguration;

    //    public AuditLoggingFilterAttribute(IAuditingConfiguration auditingConfiguration)
    //    {
    //        _auditingConfiguration = auditingConfiguration;
    //        Logger = NullLogger.Instance;
    //    }

    //    public override void OnActionExecuting(HttpActionContext actionContext)
    //    {
    //        Logger.Debug("OnActionExecuting: " + actionContext.ActionDescriptor.ActionName);
    //        base.OnActionExecuting(actionContext);
    //    }

    //    public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
    //    {
    //        Logger.Debug("OnActionExecutingAsync: " + actionContext.ActionDescriptor.ActionName);
    //        return base.OnActionExecutingAsync(actionContext, cancellationToken);
    //    }

    //    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    //    {
    //        Logger.Debug("OnActionExecuted: " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
    //        base.OnActionExecuted(actionExecutedContext);
    //    }

    //    public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
    //    {
    //        Logger.Debug("OnActionExecutedAsync: " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
    //        return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
    //    }

    //    private static MethodInfo GetMethodInfo(HttpActionDescriptor actionDescriptor)
    //    {
    //        if (actionDescriptor is ReflectedHttpActionDescriptor)
    //        {
    //            return ((ReflectedHttpActionDescriptor)actionDescriptor).MethodInfo;
    //        }

    //        return null;
    //    }
    //}
}