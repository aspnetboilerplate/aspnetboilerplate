using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Application.Features;
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
            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            await CheckFeatures(methodInfo);
            await CheckPermissions(methodInfo);
        }

        //TODO: This methods can be moved to another class to be shared.

        private async Task CheckFeatures(MethodInfo methodInfo)
        {
            var featureAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType<RequiresFeatureAttribute>(
                    methodInfo
                    );

            if (featureAttributes.Count <= 0)
            {
                return;
            }

            using (var featureChecker = _iocResolver.ResolveAsDisposable<IFeatureChecker>())
            {
                foreach (var featureAttribute in featureAttributes)
                {
                    await featureChecker.Object.CheckEnabledAsync(featureAttribute.RequiresAll, featureAttribute.Features);
                }
            }
        }

        private async Task CheckPermissions(MethodInfo methodInfo)
        {
            var authorizeAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType(
                    methodInfo
                ).OfType<IAbpAuthorizeAttribute>().ToArray();
            
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