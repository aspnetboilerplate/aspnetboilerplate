using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Authorization
{
    public class AbpAuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public AbpAuthorizationFilter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizeAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType<AbpMvcAuthorizeAttribute>(
                    context.ActionDescriptor.GetMethodInfo()
                );

            if (!authorizeAttributes.Any())
            {
                return;
            }

            using (var authorizationAttributeHelper = _iocResolver.ResolveAsDisposable<IAuthorizeAttributeHelper>())
            {
                await authorizationAttributeHelper.Object.AuthorizeAsync(authorizeAttributes);
            }
        }
    }
}