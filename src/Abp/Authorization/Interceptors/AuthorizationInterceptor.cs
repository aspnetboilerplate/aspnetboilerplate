using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Reflection;
using Abp.Threading;
using Castle.DynamicProxy;

namespace Abp.Authorization.Interceptors
{
    /// <summary>
    /// This class is used to intercept methods to make authorization if the method defined <see cref="AbpAuthorizeAttribute"/>.
    /// </summary>
    public class AuthorizationInterceptor : IInterceptor
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
            using (var authorizationAttributeHelper = _iocResolver.ResolveAsDisposable<IAuthorizeAttributeHelper>()) //TODO: Inject?
            {
                authorizationAttributeHelper.Object.Authorize(authorizeAttributes);
                invocation.Proceed();
            }

            //TODO: Async is not worked here, we will check it later. For now, using sync authorization.

            //var authorizationAttributeHelper = _iocResolver.ResolveAsDisposable<IAuthorizeAttributeHelper>();

            //if (invocation.Method.ReturnType == typeof (Task))
            //{
            //    invocation.ReturnValue = InternalAsyncHelper.InvokeWithPreAndFinalActionAsync(
            //        invocation,
            //        async () => await authorizationAttributeHelper.Object.AuthorizeAsync(authorizeAttributes),
            //        () => _iocResolver.Release(authorizationAttributeHelper)
            //        );
            //}
            //else
            //{
            //    invocation.ReturnValue = InternalAsyncHelper.CallInvokeWithPreAndFinalActionAsync(
            //        invocation.Method.ReturnType.GenericTypeArguments[0],
            //        invocation,
            //        async () => await authorizationAttributeHelper.Object.AuthorizeAsync(authorizeAttributes),
            //        () => _iocResolver.Release(authorizationAttributeHelper)
            //        );
            //}
        }

        private void InterceptSync(IInvocation invocation, IEnumerable<AbpAuthorizeAttribute> authorizeAttributes)
        {
            using (var authorizationAttributeHelper = _iocResolver.ResolveAsDisposable<IAuthorizeAttributeHelper>())
            {
                authorizationAttributeHelper.Object.Authorize(authorizeAttributes);
                invocation.Proceed();
            }
        }
    }
}
