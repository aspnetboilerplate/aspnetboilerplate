using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Filters
{
    public class AbpAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IIocResolver _iocResolver;

        public AbpAuthorizationFilter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            var authorizeAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType<AbpAuthorizeAttribute>(
                    methodInfo
                    );

            if (authorizeAttributes.Count <= 0)
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