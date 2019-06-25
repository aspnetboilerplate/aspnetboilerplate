using System.Reflection;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Extensions;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Castle.DynamicProxy;

namespace Abp.WebApi.Controllers.Dynamic.Interceptors
{
    /// <summary>
    /// Interceptor dynamic controllers.
    /// It handles method calls to a dynmaic generated api controller and 
    /// calls underlying proxied object.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    public class AbpDynamicApiControllerInterceptor<T> : CastleAbpInterceptorAdapter<AbpDynamicApiControllerInterceptor<T>>
    {
        /// <summary>
        /// Real object instance to call it's methods when dynamic controller's methods are called.
        /// </summary>
        private readonly T _proxiedObject;

        /// <summary>
        /// Creates a new AbpDynamicApiControllerInterceptor object.
        /// </summary>
        /// <param name="proxiedObject">Real object instance to call it's methods when dynamic controller's methods are called</param>
        public AbpDynamicApiControllerInterceptor(T proxiedObject)
        {
            _proxiedObject = proxiedObject;
        }

        /// <summary>
        /// Intercepts method calls of dynamic api controller
        /// </summary>
        /// <param name="invocation">Method invocation information</param>
        public void Intercept(IInvocation invocation)
        {
            //If method call is for generic type (T)...
            if (DynamicApiControllerActionHelper.IsMethodOfType(invocation.Method, typeof(T)))
            {
                //Call real object's method
                try
                {
                    invocation.ReturnValue = invocation.Method.Invoke(_proxiedObject, invocation.Arguments);
                }
                catch (TargetInvocationException targetInvocation)
                {
                    if (targetInvocation.InnerException != null)
                    {
                        targetInvocation.InnerException.ReThrow();
                    }

                    throw;
                }
            }
            else
            {
                //Call api controller's methods as usual.
                invocation.Proceed();
            }
        }

        protected bool ShouldIntercept(IAbpMethodInvocation invocation)
        {
            if (DynamicApiControllerActionHelper.IsMethodOfType(invocation.Method, typeof(T)))
            {
                return false;
            }

            return true;
        }

        protected override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                await invocation.ProceedAsync();
            }

            try
            {
                invocation.ReturnValue = invocation.Method.Invoke(_proxiedObject, invocation.Arguments);
            }
            catch (TargetInvocationException targetInvocation)
            {
                targetInvocation.InnerException?.ReThrow();

                throw;
            }
        }

        protected override void InterceptSync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                invocation.Proceed();
            }

            try
            {
                invocation.ReturnValue = invocation.Method.Invoke(_proxiedObject, invocation.Arguments);
            }
            catch (TargetInvocationException targetInvocation)
            {
                targetInvocation.InnerException?.ReThrow();

                throw;
            }
        }
    }
}