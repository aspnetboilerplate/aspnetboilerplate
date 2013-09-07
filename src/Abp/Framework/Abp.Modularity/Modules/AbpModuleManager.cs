using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Windsor;

namespace Abp.Modules
{
    public class AbpModuleManager
    {
        public static AbpModuleManager Instance { get { return _instance; } }

        public IDictionary<string, AbpModule> Modules { get; set; } //TODO: Make readyonly and dictionary?

        private static readonly AbpModuleManager _instance = new AbpModuleManager();

        public WindsorContainer IocContainer { get; set; }

        private AbpModuleManager()
        {
            Modules = new Dictionary<string, AbpModule>();
        }

        public void RegisterModule(AbpModule module)
        {
            Modules[AbpModuleHelper.GetModuleName(module)] = module;
        }

        public void PreInitializeModules(AbpInitializationContext initializationContext)
        {
            //TODO initialize in order to dependencies
            foreach (var module in Modules.Values)
            {
                module.PreInitialize(initializationContext);
            }
        }

        public void InitializeModules(AbpInitializationContext initializationContext)
        {
            //TODO initialize in order to dependencies
            foreach (var module in Modules.Values)
            {
                module.Initialize(initializationContext);
            }
        }

        public void PostInitializeModules(AbpInitializationContext initializationContext)
        {
            //TODO initialize in order to dependencies
            foreach (var module in Modules.Values)
            {
                module.PostInitialize(initializationContext);
            }
        }
    }

    public class AbpInitializationContext
    {
        public WindsorContainer IocContainer { get; private set; }

        public AbpInitializationContext(WindsorContainer iocContainer)
        {
            IocContainer = iocContainer;
        }
    }

    public static class AbpModuleHelper
    {
        public static string GetModuleName(AbpModule module)
        {
            return GetModuleName(module.GetType());
        }

        public static string GetModuleName<TModule>() where TModule: AbpModule
        {
            return GetModuleName(typeof (TModule));
        } 

        private static string GetModuleName(Type type)
        {
            //TODO: Check attr and throw exception
            var moduleAttribute = ReflectionHelper.GetSingleAttribute<AbpModuleAttribute>(type);
            return moduleAttribute.Name;
        }
    }

    /// <summary>
    /// This class is used to perform some common reflection related operations.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets a single attrbiute for a member.
        /// </summary>
        /// <typeparam name="T">Type of the attribute</typeparam>
        /// <param name="memberInfo">The member that will be checked for the attribute</param>
        /// <param name="inherit">Include inherited attrbiutes</param>
        /// <returns>Returns the attribute object if found. Returns null if not found.</returns>
        public static T GetSingleAttribute<T>(MemberInfo memberInfo, bool inherit = true) where T : class
        {
            var attrs = memberInfo.GetCustomAttributes(typeof(T), inherit);
            if (attrs.Length > 0)
            {
                return (T)attrs[0];
            }

            return default(T);
        }
    }
}