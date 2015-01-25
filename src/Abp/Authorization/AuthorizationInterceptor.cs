using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Reflection;
using Abp.Threading;
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
            var authorizeAttrList =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType<AbpAuthorizeAttribute>(
                    invocation.MethodInvocationTarget
                    );

            if (authorizeAttrList.Count <= 0)
            {
                //No AbpAuthorizeAttribute to be checked
                invocation.Proceed();
                return;
            }

            if (AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                InterceptAsync(invocation, authorizeAttrList);
            }
            else
            {
                InterceptSync(invocation, authorizeAttrList);
            }
        }

        private void InterceptAsync(IInvocation invocation, IEnumerable<AbpAuthorizeAttribute> authorizeAttributes)
        {
            var authorizationAttributeHelper = _iocResolver.ResolveAsDisposable<IAuthorizeAttributeHelper>();
            invocation.ReturnValue = InvokeWithPreAndFinalActionAsync(
                invocation,
                async () => await authorizationAttributeHelper.Object.AuthorizeAsync(authorizeAttributes),
                () => _iocResolver.Release(authorizationAttributeHelper)
                );
        }

        private void InterceptSync(IInvocation invocation, IEnumerable<AbpAuthorizeAttribute> authorizeAttributes)
        {
            using (var authorizationAttributeHelper = _iocResolver.ResolveAsDisposable<IAuthorizeAttributeHelper>())
            {
                authorizationAttributeHelper.Object.Authorize(authorizeAttributes);
                invocation.Proceed();
            }
        }

        private static async Task InvokeWithPreAndFinalActionAsync(IInvocation invocation, Func<Task> preAction, Action finalAction)
        {
            try
            {
                await preAction();
                invocation.Proceed();
                await (Task)invocation.ReturnValue;
            }
            finally
            {
                finalAction();
            }
        }
    }
}
