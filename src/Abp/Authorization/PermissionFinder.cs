using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Abp.Authorization
{
    /// <summary>
    /// This class is used to get permissions out of the system.
    /// Normally, you should inject and use <see cref="IPermissionManager"/>.
    /// But this class can be used in database migrations or in unit tests where Abp is not initialized.
    /// </summary>
    public static class PermissionFinder
    {
        /// <summary>
        /// Collects and gets all permissions in given providers.
        /// </summary>
        /// <param name="providerTypes">Providers</param>
        /// <returns>List of permissions</returns>
        public static IReadOnlyList<Permission> GetAllPermissions(params Type[] providerTypes)
        {
            return new InternalPermissionFinder(providerTypes).GetAllPermissions();
        }

        internal class InternalPermissionFinder : PermissionDefinitionContextBase
        {
            public InternalPermissionFinder(params Type[] providerTypes)
            {
                foreach (var authorizationProviderType in providerTypes)
                {
                    var provider = (AuthorizationProvider)Activator.CreateInstance(authorizationProviderType);
                    provider.SetPermissions(this);
                }

                Permissions.AddAllPermissions();
            }

            public IReadOnlyList<Permission> GetAllPermissions()
            {
                return Permissions.Values.ToImmutableList();
            }
        }
    }
}