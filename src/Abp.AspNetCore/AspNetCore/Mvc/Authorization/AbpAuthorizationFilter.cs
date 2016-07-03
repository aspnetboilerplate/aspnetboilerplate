using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Authorization;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Authorization
{
    public class AbpAuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
    {
        private readonly IAuthorizationHelper _authorizationHelper;

        public AbpAuthorizationFilter(IAuthorizationHelper authorizationHelper)
        {
            _authorizationHelper = authorizationHelper;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await _authorizationHelper.AuthorizeAsync(context.ActionDescriptor.GetMethodInfo());
        }
    }
}