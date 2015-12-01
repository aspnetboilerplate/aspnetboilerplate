using System;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Castle.Core;
using Castle.MicroKernel;

namespace Abp.Application.Features
{
    /// <summary>
    /// Used to register <see cref="FeatureInterceptor"/> for needed classes.
    /// </summary>
    internal static class FeatureInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        private static void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            if (ShouldIntercept(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(FeatureInterceptor)));
            }
        }

        private static bool ShouldIntercept(Type type)
        {
            if (type.IsDefined(typeof(RequiresFeatureAttribute), true))
            {
                return true;
            }

            if (type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(m => m.IsDefined(typeof(RequiresFeatureAttribute), true)))
            {
                return true;
            }

            return false;
        }
    }
}
