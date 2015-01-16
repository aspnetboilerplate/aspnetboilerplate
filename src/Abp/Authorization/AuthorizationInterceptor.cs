using Abp.Dependency;
using Abp.Reflection;
using Castle.DynamicProxy;

namespace Abp.Authorization
{
    /// <summary>
    /// This class is used to intercept methods to make authorization if the method defined <see cref="AbpAuthorizeAttribute"/>.
    /// </summary>
    internal class AuthorizationInterceptor : IInterceptor
    {
        private readonly IIocResolver _iocResolver;

        public AuthorizationInterceptor(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public void Intercept(IInvocation invocation)
        {
            Authorize(invocation);
            invocation.Proceed();
        }

        public void Authorize(IInvocation invocation)
        {
            var authorizeAttrList =
                ReflectionHelper
                    .GetAttributesOfMemberAndDeclaringType<AbpAuthorizeAttribute>(
                        invocation.MethodInvocationTarget
                    );

            if (authorizeAttrList.Count <= 0)
            {
                return;
            }

            using (var authorizationAttributeHelper = _iocResolver.ResolveAsDisposable<AuthorizeAttributeHelper>())
            {
                authorizationAttributeHelper.Object.Authorize(authorizeAttrList);
            }
        }
    }
}
