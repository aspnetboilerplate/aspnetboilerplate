using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// Default implementation of <see cref="IPermissionDefinitionProviderFinder"/>.
    /// </summary>
    public class DefaultPermissionDefinitionProviderFinder : IPermissionDefinitionProviderFinder, ITransientDependency
    {
        public IAssemblyFinder AssemblyFinder { get; set; }

        public DefaultPermissionDefinitionProviderFinder()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
        }

        public virtual IEnumerable<IPermissionDefinitionProvider> GetPermissionProviders()
        {
            return
                from assembly in AssemblyFinder.GetAllAssemblies()
                from type in assembly.GetTypes()
                where typeof (IPermissionDefinitionProvider).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract
                select (IPermissionDefinitionProvider) Activator.CreateInstance(type);
        }
    }
}