using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Authorization;
using Abp.Dependency;
using Microsoft.AspNetCore.Authorization;
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
            //Check IAllowAnonymous
            if (context.ActionDescriptor
                .GetMethodInfo()
                .GetCustomAttributes(true)
                .OfType<IAllowAnonymous>()
                .Any())
            {
                return;
            }

            await _authorizationHelper.AuthorizeAsync(context.ActionDescriptor.GetMethodInfo());
        }
    }
}