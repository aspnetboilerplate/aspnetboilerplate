using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.Mvc.Results;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Web.Models;
using Abp.Web.Mvc.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Authorization
{
    public class AbpAuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly IErrorInfoBuilder _errorInfoBuilder;

        public AbpAuthorizationFilter(
            IAuthorizationHelper authorizationHelper,
            IErrorInfoBuilder errorInfoBuilder
            )
        {
            _authorizationHelper = authorizationHelper;
            _errorInfoBuilder = errorInfoBuilder;
            Logger = NullLogger.Instance;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Allow Anonymous skips all authorization
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            try
            {
                //TODO: Avoid using try/catch, use conditional checking
                await _authorizationHelper.AuthorizeAsync(context.ActionDescriptor.GetMethodInfo());
            }
            catch (AbpAuthorizationException ex)
            {
                Logger.Warn(ex.ToString(), ex);

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new MvcAjaxResponse(_errorInfoBuilder.BuildForException(ex), true))
                    {
                        StatusCode = context.HttpContext.User.Identity.IsAuthenticated
                            ? (int) System.Net.HttpStatusCode.Forbidden
                            : (int) System.Net.HttpStatusCode.Unauthorized
                    };
                }
                else
                {
                    context.Result = new ChallengeResult();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new MvcAjaxResponse(_errorInfoBuilder.BuildForException(ex)))
                    {
                        StatusCode = (int) System.Net.HttpStatusCode.InternalServerError
                    };
                }
                else
                {
                    //TODO: How to return Error page?
                    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}