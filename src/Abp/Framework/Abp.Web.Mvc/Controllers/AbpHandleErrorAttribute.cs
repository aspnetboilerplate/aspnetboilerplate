using System;
using System.Web;
using System.Web.Mvc;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Logging;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers.Results;
using Abp.Web.Mvc.Models;

namespace Abp.Web.Mvc.Controllers
{
    /// <summary>
    /// Used internally by ABP to handle exceptions on MVC actions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AbpHandleErrorAttribute : HandleErrorAttribute /* This class is written by looking at the source codes of System.Web.Mvc.HandleErrorAttribute class */
    {
        public override void OnException(ExceptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.ExceptionHandled || context.IsChildAction)
            {
                return;
            }

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (!context.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            var exception = context.Exception;
            
            LogHelper.LogException(exception);

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
            // ignore it.
            if (new HttpException(null, exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(exception))
            {
                return;
            }

            context.ExceptionHandled = true;
            context.HttpContext.Response.Clear();
            context.Result = IsAjaxRequest(context)
                ? GenerateAjaxResult(context)
                : GenerateNonAjaxResult(context);

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            context.HttpContext.Response.TrySkipIisCustomErrors = true;

            EventBus.Default.Trigger(this, new AbpHandledExceptionData(context.Exception));
        }

        private bool IsAjaxRequest(ExceptionContext context)
        {
            return context.HttpContext.Request.IsAjaxRequest();
        }

        private ActionResult GenerateAjaxResult(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = 200;
            return new AbpJsonResult(new MvcAjaxResponse(ErrorInfo.ForException(context.Exception)));
        }

        private ActionResult GenerateNonAjaxResult(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = 500;
            return new ViewResult
                   {
                       ViewName = View,
                       MasterName = Master,
                       ViewData = new ViewDataDictionary<ErrorViewModel>(new ErrorViewModel(context.Exception)),
                       TempData = context.Controller.TempData
                   };
        }
    }
}