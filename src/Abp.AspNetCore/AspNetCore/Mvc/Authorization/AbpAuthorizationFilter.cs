using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Authorization;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Authorization
{
    //TODO: Register only actions which define AbpMvcAuthorizeAttribute..?
    public class AbpAuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
    {
        private readonly IAuthorizationHelper _authorizationHelper;

        public AbpAuthorizationFilter(IAuthorizationHelper authorizationHelper)
        {
            _authorizationHelper = authorizationHelper;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Allow Anonymous skips all authorization
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            await _authorizationHelper.AuthorizeAsync(context.ActionDescriptor.GetMethodInfo());
        }
    }
}