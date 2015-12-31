using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Logging;
using Abp.Web.Models;
using Castle.Core.Logging;

namespace Abp.WebApi.Controllers.Filters
{
    /// <summary>
    /// Used to handle exceptions on web api controllers.
    /// </summary>
    public class AbpExceptionFilterAttribute : ExceptionFilterAttribute, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        public AbpExceptionFilterAttribute()
        {
            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            var wrapAttr = GetWrapAttributeOrNull(context) ?? WrapResultAttribute.Default;
            
            if (wrapAttr.LogError)
            {
                LogHelper.LogException(Logger, context.Exception);                
            }

            if (wrapAttr.OnError)
            {
                context.Response = context.Request.CreateResponse(
                    HttpStatusCode.OK,
                    new AjaxResponse(
                        ErrorInfoBuilder.Instance.BuildForException(context.Exception),
                        context.Exception is Abp.Authorization.AbpAuthorizationException)
                    );

                EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));
            }
        }

        private static WrapResultAttribute GetWrapAttributeOrNull(HttpActionExecutedContext context)
        {
            //Try to get for dynamic APIs
            var dontWrapAttr = context.ActionContext.ActionDescriptor.Properties["__AbpDynamicApiDontWrapResultAttribute"] as WrapResultAttribute;
            if (dontWrapAttr != null)
            {
                return dontWrapAttr;
            }

            //Get for the action
            dontWrapAttr = context.ActionContext.ActionDescriptor.GetCustomAttributes<WrapResultAttribute>().FirstOrDefault();
            if (dontWrapAttr != null)
            {
                return dontWrapAttr;
            }

            //Get for the controller
            dontWrapAttr = context.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<WrapResultAttribute>().FirstOrDefault();
            if (dontWrapAttr != null)
            {
                return dontWrapAttr;
            }

            //Not found
            return null;
        }
    }
}