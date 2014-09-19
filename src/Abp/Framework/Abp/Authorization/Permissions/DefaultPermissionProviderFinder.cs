using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Authorization.Permissions
{
    /// <summary>
    /// Default implementation of <see cref="IPermissionProviderFinder"/>.
    /// </summary>
    public class DefaultPermissionProviderFinder : IPermissionProviderFinder, ITransientDependency
    {
        public IAssemblyFinder AssemblyFinder { get; set; }

        public DefaultPermissionProviderFinder()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
        }

        public virtual IEnumerable<IPermissionProvider> GetPermissionProviders()
        {
            return
                from assembly in AssemblyFinder.GetAllAssemblies()
                from type in assembly.GetTypes()
                where typeof (IPermissionProvider).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract
                select (IPermissionProvider) Activator.CreateInstance(type);
        }
    }
}