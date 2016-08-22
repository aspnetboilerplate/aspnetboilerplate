using System.Net;
using System.Reflection;
using System.Web.Mvc;
using Abp.Dependency;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers.Results;
using Abp.Web.Mvc.Extensions;
using Abp.Web.Mvc.Helpers;
using Abp.Web.Security.AntiForgery;
using Castle.Core.Logging;

namespace Abp.Web.Mvc.Security.AntiForgery
{
    public class AbpAntiForgeryMvcFilter: IAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IAbpAntiForgeryManager _abpAntiForgeryManager;

        public AbpAntiForgeryMvcFilter(IAbpAntiForgeryManager abpAntiForgeryManager)
        {
            _abpAntiForgeryManager = abpAntiForgeryManager;
            Logger = NullLogger.Instance;
        }

        public void OnAuthorization(AuthorizationContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return;
            }

            var httpVerb = HttpVerbHelper.Create(context.HttpContext.Request.HttpMethod);
            if (!_abpAntiForgeryManager.ShouldValidate(methodInfo, httpVerb, true))
            {
                return;
            }

            if (!_abpAntiForgeryManager.IsValid(context.HttpContext))
            {
                CreateForbiddenResponse(context, methodInfo, "Empty or invalid anti forgery header token.");
            }
        }

        private void CreateForbiddenResponse(
            AuthorizationContext context, 
            MethodInfo methodInfo, 
            string message)
        {
            Logger.Warn(message);
            Logger.Warn("Requested URI: " + context.HttpContext.Request.Url);

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.HttpContext.Response.StatusDescription = message;

            var isJsonResult = MethodInfoHelper.IsJsonResult(methodInfo);

            if (isJsonResult)
            {
                context.Result = CreateUnAuthorizedJsonResult(message);
            }
            else
            {
                context.Result = CreateUnAuthorizedNonJsonResult(context, message);
            }

            if (isJsonResult || context.HttpContext.Request.IsAjaxRequest())
            {
                context.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            }
        }

        protected virtual AbpJsonResult CreateUnAuthorizedJsonResult(string message)
        {
            return new AbpJsonResult(new AjaxResponse(new ErrorInfo(message), true))
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        protected virtual HttpStatusCodeResult CreateUnAuthorizedNonJsonResult(AuthorizationContext filterContext, string message)
        {
            return new HttpStatusCodeResult(filterContext.HttpContext.Response.StatusCode, message);
        }
    }
}
