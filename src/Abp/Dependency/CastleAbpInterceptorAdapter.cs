using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Threading;
using Castle.DynamicProxy;

namespace Abp.Dependency
{
    public abstract class CastleAbpInterceptorAdapter<TInterceptor> : IInterceptor
    {
        private static readonly MethodInfo MethodExecuteWithoutReturnValueAsync =
            typeof(CastleAbpInterceptorAdapter<TInterceptor>)
                .GetMethod(
                    nameof(ExecuteWithoutReturnValueAsync),
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

        private static readonly MethodInfo MethodExecuteWithReturnValueAsync =
            typeof(CastleAbpInterceptorAdapter<TInterceptor>)
                .GetMethod(
                    nameof(ExecuteWithReturnValueAsync),
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

        public void Intercept(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            var method = invocation.MethodInvocationTarget ?? invocation.Method;

            if (method.IsAsync())
            {
                InterceptAsyncMethod(invocation, proceedInfo);
            }
            else
            {
                InterceptSyncMethod(invocation, proceedInfo);
            }
        }

        protected abstract void InterceptSync(IAbpMethodInvocation invocationMethodInvocation);

        protected virtual Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            InterceptSync(invocation);
            return Task.CompletedTask;
        }

        private void InterceptSyncMethod(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            InterceptSync(new CastleAbpMethodInvocationAdapter(invocation, proceedInfo));
        }

        private void InterceptAsyncMethod(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = MethodExecuteWithoutReturnValueAsync
                    .Invoke(this, new object[] { invocation, proceedInfo });
            }
            else
            {
                invocation.ReturnValue = MethodExecuteWithReturnValueAsync
                    .MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0])
                    .Invoke(this, new object[] { invocation, proceedInfo });
            }
        }

        private async Task ExecuteWithoutReturnValueAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            await Task.Yield();

            await InterceptAsync(
                new CastleAbpMethodInvocationAdapter(invocation, proceedInfo)
            );
        }

        private async Task<T> ExecuteWithReturnValueAsync<T>(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            await Task.Yield();

            await InterceptAsync(
                new CastleAbpMethodInvocationAdapter(invocation, proceedInfo)
            );

            return await (Task<T>)invocation.ReturnValue;
        }
    }

    public interface IAbpMethodInvocation
    {
        object[] Arguments { get; }

        IReadOnlyDictionary<string, object> ArgumentsDictionary { get; }

        Type[] GenericArguments { get; }

        object TargetObject { get; }

        Type TargetObjectType { get; }

        MethodInfo Method { get; }

        object ReturnValue { get; set; }

        void Proceed();

        Task ProceedAsync();
    }

    public class CastleAbpMethodInvocationAdapter : IAbpMethodInvocation
    {
        public object[] Arguments => Invocation.Arguments;

        public IReadOnlyDictionary<string, object> ArgumentsDictionary => _lazyArgumentsDictionary.Value;
        private readonly Lazy<IReadOnlyDictionary<string, object>> _lazyArgumentsDictionary;

        public Type[] GenericArguments => Invocation.GenericArguments;

        public object TargetObject => Invocation.InvocationTarget ?? Invocation.MethodInvocationTarget;

        public Type TargetObjectType => Invocation.TargetType;

        public MethodInfo Method => Invocation.MethodInvocationTarget ?? Invocation.Method;

        public object ReturnValue
        {
            get => _actualReturnValue ?? Invocation.ReturnValue;
            set => Invocation.ReturnValue = value;
        }

        private object _actualReturnValue;

        protected IInvocation Invocation { get; }
        protected IInvocationProceedInfo ProceedInfo { get; }

        public CastleAbpMethodInvocationAdapter(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            Invocation = invocation;
            ProceedInfo = proceedInfo;

            _lazyArgumentsDictionary = new Lazy<IReadOnlyDictionary<string, object>>(GetArgumentsDictionary);
        }

        public void Proceed()
        {
            ProceedInfo.Invoke();

            if (Invocation.Method.IsAsync())
            {
                AsyncHelper.RunSync(() => (Task)Invocation.ReturnValue);
            }
        }

        public Task ProceedAsync()
        {
            ProceedInfo.Invoke();

            _actualReturnValue = Invocation.ReturnValue;

            return Invocation.Method.IsAsync()
                ? (Task)_actualReturnValue
                : Task.FromResult(_actualReturnValue);
        }

        private IReadOnlyDictionary<string, object> GetArgumentsDictionary()
        {
            var dict = new Dictionary<string, object>();

            var methodParameters = Method.GetParameters();
            for (int i = 0; i < methodParameters.Length; i++)
            {
                dict[methodParameters[i].Name] = Invocation.Arguments[i];
            }

            return dict;
        }
    }
}
