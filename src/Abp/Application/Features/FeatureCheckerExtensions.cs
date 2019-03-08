using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Threading;

namespace Abp.Application.Features
{
    /// <summary>
    /// Some extension methods for <see cref="IFeatureChecker"/>.
    /// </summary>
    public static class FeatureCheckerExtensions
    {
        /// <summary>
        /// Gets the value of a feature by its name. This is the sync version of <see cref="IFeatureChecker.GetValueAsync(string)"/>
        /// 
        /// This is a shortcut for <see cref="GetValue(IFeatureChecker, int, string)"/> that uses <see cref="IAbpSession.TenantId"/>.
        /// Note: This method should be used only if the TenantId can be obtained from the session.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="featureName">Unique feature name</param>
        /// <returns>Feature's current value</returns>
        public static string GetValue(this IFeatureChecker featureChecker, string featureName)
        {
            return AsyncHelper.RunSync(() => featureChecker.GetValueAsync(featureName));
        }

        /// <summary>
        /// Gets the value of a feature by its name. This is the sync version of <see cref="IFeatureChecker.GetValueAsync(int, string)"/>
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="tenantId">Tenant's Id</param>
        /// <param name="featureName">Unique feature name</param>
        /// <returns>Feature's current value</returns>
        public static string GetValue(this IFeatureChecker featureChecker, int tenantId, string featureName)
        {
            return AsyncHelper.RunSync(() => featureChecker.GetValueAsync(tenantId, featureName));
        }

        /// <summary>
        /// Checks if a given feature is enabled.
        /// This should be used for boolean-value features.
        /// 
        /// This is a shortcut for <see cref="IsEnabled(IFeatureChecker, int, string)"/> that uses <see cref="IAbpSession.TenantId"/>.
        /// Note: This method should be used only if the TenantId can be obtained from the session.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="name">Unique feature name</param>
        /// <returns>True, if the current feature's value is "true".</returns>
        public static bool IsEnabled(this IFeatureChecker featureChecker, string name)
        {
            return AsyncHelper.RunSync(() => featureChecker.IsEnabledAsync(name));
        }

        /// <summary>
        /// Checks if a given feature is enabled.
        /// This should be used for boolean-value features.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="tenantId">Tenant's Id</param>
        /// <param name="featureName">Unique feature name</param>
        /// <returns>True, if the current feature's value is "true".</returns>
        public static bool IsEnabled(this IFeatureChecker featureChecker, int tenantId, string featureName)
        {
            return AsyncHelper.RunSync(() => featureChecker.IsEnabledAsync(tenantId, featureName));
        }

        /// <summary>
        /// Used to check if one or all of the given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static async Task<bool> IsEnabledAsync(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!(await featureChecker.IsEnabledAsync(featureName)))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var featureName in featureNames)
            {
                if (await featureChecker.IsEnabledAsync(featureName))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Used to check if one or all of the given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static async Task<bool> IsEnabledAsync(this IFeatureChecker featureChecker, int tenantId, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!(await featureChecker.IsEnabledAsync(tenantId, featureName)))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var featureName in featureNames)
            {
                if (await featureChecker.IsEnabledAsync(tenantId, featureName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Used to check if one or all of the given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static bool IsEnabled(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            return AsyncHelper.RunSync(() => featureChecker.IsEnabledAsync(requiresAll, featureNames));
        }

        /// <summary>
        /// Used to check if one or all of the given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static bool IsEnabled(this IFeatureChecker featureChecker, int tenantId, bool requiresAll, params string[] featureNames)
        {
            return AsyncHelper.RunSync(() => featureChecker.IsEnabledAsync(tenantId, requiresAll, featureNames));
        }

        /// <summary>
        /// Checks if a given feature is enabled. Throws <see cref="AbpAuthorizationException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="featureName">Unique feature name</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, string featureName)
        {
            if (!(await featureChecker.IsEnabledAsync(featureName)))
            {
                throw new AbpAuthorizationException("Feature is not enabled: " + featureName);
            }
        }

        /// <summary>
        /// Checks if a given feature is enabled. Throws <see cref="AbpAuthorizationException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="featureName">Unique feature name</param>
        public static void CheckEnabled(this IFeatureChecker featureChecker, string featureName)
        {
            if (!featureChecker.IsEnabled(featureName))
            {
                throw new AbpAuthorizationException("Feature is not enabled: " + featureName);
            }
        }

        /// <summary>
        /// Checks if one or all of the given features are enabled. Throws <see cref="AbpAuthorizationException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!(await featureChecker.IsEnabledAsync(featureName)))
                    {
                        throw new AbpAuthorizationException(
                            "Required features are not enabled. All of these features must be enabled: " +
                            string.Join(", ", featureNames)
                            );
                    }
                }
            }
            else
            {
                foreach (var featureName in featureNames)
                {
                    if (await featureChecker.IsEnabledAsync(featureName))
                    {
                        return;
                    }
                }

                throw new AbpAuthorizationException(
                    "Required features are not enabled. At least one of these features must be enabled: " +
                    string.Join(", ", featureNames)
                    );
            }
        }

        /// <summary>
        /// Checks if one or all of the given features are enabled. Throws <see cref="AbpAuthorizationException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, int tenantId, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!(await featureChecker.IsEnabledAsync(tenantId, featureName)))
                    {
                        throw new AbpAuthorizationException(
                            "Required features are not enabled. All of these features must be enabled: " +
                            string.Join(", ", featureNames)
                            );
                    }
                }
            }
            else
            {
                foreach (var featureName in featureNames)
                {
                    if (await featureChecker.IsEnabledAsync(tenantId, featureName))
                    {
                        return;
                    }
                }

                throw new AbpAuthorizationException(
                    "Required features are not enabled. At least one of these features must be enabled: " +
                    string.Join(", ", featureNames)
                    );
            }
        }

        /// <summary>
        /// Checks if one or all of the given features are enabled. Throws <see cref="AbpAuthorizationException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static void CheckEnabled(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            AsyncHelper.RunSync(() => featureChecker.CheckEnabledAsync(requiresAll, featureNames));
        }

        /// <summary>
        /// Checks if one or all of the given features are enabled. Throws <see cref="AbpAuthorizationException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static void CheckEnabled(this IFeatureChecker featureChecker, int tenantId, bool requiresAll, params string[] featureNames)
        {
            AsyncHelper.RunSync(() => featureChecker.CheckEnabledAsync(tenantId, requiresAll, featureNames));
        }
    }
}