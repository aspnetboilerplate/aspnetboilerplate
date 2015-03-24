using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Abp.Threading
{
    internal static class InternalAsyncHelper
    {
        public static async Task WaitTaskAndActionWithFinally(Task actualReturnValue, Func<Task> afterAction, Action finalAction)
        {
            try
            {
                await actualReturnValue;
                await afterAction();
            }
            finally
            {
                finalAction();
            }
        }

        public static object CallReturnGenericTaskAfterAction(Type taskReturnType, object actualReturnValue, Func<Task> action, Action finalAction)
        {
            return typeof (InternalAsyncHelper)
                .GetMethod("ReturnGenericTaskAfterAction", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] { actualReturnValue, action, finalAction });
        }

        public static async Task<T> ReturnGenericTaskAfterAction<T>(Task<T> actualReturnValue, Func<Task> action, Action finalAction)
        {
            try
            {
                var response = await actualReturnValue;
                await action();
                return response;
            }
            finally
            {
                finalAction();
            }
        }

        public static async Task InvokeWithPreAndFinalActionAsync(IInvocation invocation, Func<Task> preAction, Action finalAction)
        {
            try
            {
                await preAction();
                invocation.Proceed();
                //await (Task)invocation.ReturnValue;
            }
            finally
            {
                finalAction();
            }
        }

        public static object CallInvokeWithPreAndFinalActionAsync(Type taskReturnType, IInvocation invocation, Func<Task> preAction, Action finalAction)
        {
            return typeof(InternalAsyncHelper)
                .GetMethod("GenericInvokeWithPreAndFinalActionAsync", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] { invocation, preAction, finalAction });
        }

        public static async Task<T> GenericInvokeWithPreAndFinalActionAsync<T>(IInvocation invocation, Func<Task> preAction, Action finalAction)
        {
            try
            {
                await preAction();
                invocation.Proceed();
                return await (Task<T>)invocation.ReturnValue;
            }
            finally
            {
                finalAction();
            }
        }
    }
}