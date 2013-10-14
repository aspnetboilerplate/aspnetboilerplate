using System;
using System.Web.Mvc;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers.Results;
using Abp.Web.Mvc.Models;

namespace Abp.Web.Mvc.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AbpHandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        private const string DefaultView = "Error";

        public string View { get; set; }

        public string Master { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool HandleErrors { get; set; }

        public AbpHandleErrorAttribute()
        {
            Master = String.Empty;
            View = DefaultView;
            HandleErrors = true;
        }
        
        public void OnException(ExceptionContext context)
        {
            if (!HandleErrors)
            {
                return;
            }

            if (context.HttpContext.Request.IsAjaxRequest())
            {
                HandleAjaxError(context);
            }
            else
            {
                HandleNonAjaxError(context);
            }

            context.ExceptionHandled = true;
            context.HttpContext.Response.Clear();
        }

        private void HandleAjaxError(ExceptionContext context)
        {
            context.Result = new JsonCamelCaseResult
                                 {
                                     Data = new AbpMvcAjaxResult(new AbpErrorInfo(context.Exception.Message))
                                 };
        }

        private void HandleNonAjaxError(ExceptionContext context)
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
            if (!context.HttpContext.IsCustomErrorEnabled) //TODO: To check or not to check this?
            {
                return;
            }

            context.Result = new ViewResult
                                 {
                                     ViewName = View,
                                     MasterName = Master,
                                     ViewData = new ViewDataDictionary<AbpErrorInfo>(new AbpErrorInfo(context.Exception.Message)),
                                     TempData = context.Controller.TempData
                                 };

            context.ExceptionHandled = true;
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = 500;

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            context.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}