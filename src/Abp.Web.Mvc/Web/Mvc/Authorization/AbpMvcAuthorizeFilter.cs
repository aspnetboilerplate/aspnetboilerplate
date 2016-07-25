using System.Net;
using System.Reflection;
using System.Web.Mvc;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Logging;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers.Results;
using Abp.Web.Mvc.Extensions;
using Abp.Web.Mvc.Helpers;

namespace Abp.Web.Mvc.Authorization
{
    public class AbpMvcAuthorizeFilter : IAuthorizationFilter, ITransientDependency
    {
        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly IErrorInfoBuilder _errorInfoBuilder;

        public AbpMvcAuthorizeFilter(IAuthorizationHelper authorizationHelper, IErrorInfoBuilder errorInfoBuilder)
        {
            _authorizationHelper = authorizationHelper;
            _errorInfoBuilder = errorInfoBuilder;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var methodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return;
            }

            try
            {
                _authorizationHelper.Authorize(methodInfo);
            }
            catch (AbpAuthorizationException ex)
            {
                LogHelper.Logger.Warn(ex.ToString(), ex);
                HandleUnauthorizedRequest(filterContext, methodInfo, ex);
            }
        }

        private void HandleUnauthorizedRequest(
            AuthorizationContext filterContext, 
            MethodInfo methodInfo, 
            AbpAuthorizationException ex)
        {
            filterContext.HttpContext.Response.StatusCode =
                filterContext.RequestContext.HttpContext.User?.Identity?.IsAuthenticated ?? false
                    ? (int) HttpStatusCode.Forbidden
                    : (int) HttpStatusCode.Unauthorized;

            var isJsonResult = MethodInfoHelper.IsJsonResult(methodInfo);

            if (isJsonResult)
            {
                filterContext.Result = new AbpJsonResult(
                    new AjaxResponse(
                        _errorInfoBuilder.BuildForException(ex)
                    )
                ) {JsonRequestBehavior = JsonRequestBehavior.AllowGet};
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(filterContext.HttpContext.Response.StatusCode, ex.Message);
            }

            if (isJsonResult || filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            }
        }
    }
}