using System;
using System.Reflection;
using System.Threading.Tasks;

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
                .Invoke(null, new[] { actualReturnValue, action, finalAction });
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
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
    }
}