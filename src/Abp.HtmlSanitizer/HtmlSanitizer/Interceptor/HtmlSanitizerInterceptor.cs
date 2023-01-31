using System.Reflection;
using Abp.Dependency;
using Castle.DynamicProxy;

namespace Abp.HtmlSanitizer.HtmlSanitizer.Interceptor
{
    /// <summary>
    /// This interceptor is used intercept method calls for classes which's methods must be validated.
    /// </summary>
    public class HtmlSanitizerInterceptor : AbpInterceptorBase, ITransientDependency
    {
        public HtmlSanitizerInterceptor() 
        {
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            try
            {
                var method = GetMethodInfo(invocation);
                var htmlSanitizerAttr = GetSanitizeHtmlAttributeOrNull(method);

                if (htmlSanitizerAttr == null || htmlSanitizerAttr.IsDisabled)
                {
                    invocation.Proceed();
                    return;
                }
            
                invocation.Proceed();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        protected override async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            try
            {
                var proceedInfo = invocation.CaptureProceedInfo();
                var method = GetMethodInfo(invocation);
                var htmlSanitizerAttr = GetSanitizeHtmlAttributeOrNull(method);
            
                if (htmlSanitizerAttr == null || htmlSanitizerAttr.IsDisabled)
                {
                    proceedInfo.Invoke();
                    var task = (Task)invocation.ReturnValue;
                    await task;
                    return;
                }
            
                proceedInfo.Invoke();
                await ((Task)invocation.ReturnValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        protected override async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            try
            {
                var proceedInfo = invocation.CaptureProceedInfo();
                var method = GetMethodInfo(invocation);
                var htmlSanitizerAttr = GetSanitizeHtmlAttributeOrNull(method);
            
                if (htmlSanitizerAttr == null || htmlSanitizerAttr.IsDisabled)
                {
                    proceedInfo.Invoke();
                    var taskResult = (Task<TResult>)invocation.ReturnValue;
                    return await taskResult;
                }
            
                proceedInfo.Invoke();
                return await (Task<TResult>)invocation.ReturnValue;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        
        private static SanitizeHtmlAttribute? GetSanitizeHtmlAttributeOrNull(MethodInfo methodInfo)
        {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<SanitizeHtmlAttribute>().ToArray();
            if (attrs.Length > 0)
            {
                return attrs[0];
            }

            if (methodInfo.DeclaringType != null)
                attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<SanitizeHtmlAttribute>()
                    .ToArray();

            return attrs.Length > 0 ? attrs[0] : null;
        }
        
        private static MethodInfo GetMethodInfo(IInvocation invocation)
        {
            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }

            return method;
        }
    }
}
