using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Authorization
{
    internal class PermissionProviderFinder : IPermissionProviderFinder, ITransientDependency
    {
        public IAssemblyFinder AssemblyFinder { get; set; }

        public PermissionProviderFinder()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
        }

        public virtual List<Type> FindAll()
        {
            return
                (from assembly in AssemblyFinder.GetAllAssemblies()
                from type in assembly.GetTypes()
                where IsPermissionProviderClass(type)
                select type).ToList();
        }

        private static bool IsPermissionProviderClass(Type type)
        {
            return typeof (IPermissionProvider).IsAssignableFrom(type)
                   && type.IsClass
                   && !type.IsAbstract;
        }
    }
}