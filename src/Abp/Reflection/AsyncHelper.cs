using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.Reflection
{
    internal static class AsyncHelper
    {
        public static async Task ReturnTaskAfterAction(Task actualReturnValue, Func<Task> action)
        {
            await actualReturnValue;
            await action();
        }

        public static object CallReturnAfterAction(Type taskReturnType, object actualReturnValue, Func<Task> action)
        {
            return typeof (AsyncHelper)
                .GetMethod("ReturnGenericTaskAfterAction", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(taskReturnType)
                .Invoke(null, new[] {actualReturnValue, action});
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }

        public static async Task<T> ReturnGenericTaskAfterAction<T>(Task<T> actualReturnValue, Func<Task> action)
        {
            var response = await actualReturnValue;
            await action();
            return response;
        }
    }
}