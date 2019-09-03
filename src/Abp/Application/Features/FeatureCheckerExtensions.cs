using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Localization;
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
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static bool IsEnabled(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!featureChecker.IsEnabled(featureName))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var featureName in featureNames)
            {
                if (featureChecker.IsEnabled(featureName))
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
        /// <param name="tenantId">Tenant id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static bool IsEnabled(this IFeatureChecker featureChecker, int tenantId, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!featureChecker.IsEnabled(tenantId, featureName))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var featureName in featureNames)
            {
                if (featureChecker.IsEnabled(tenantId, featureName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a given feature is enabled. Throws <see cref="AbpAuthorizationException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="featureName">Unique feature name</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, string featureName)
        {
            var localizedFeatureNames = LocalizeFeatureNames(featureChecker, new []{ featureName });

            if (!(await featureChecker.IsEnabledAsync(featureName)))
            {
                throw new AbpAuthorizationException(string.Format(
                    L(
                        featureChecker,
                        "FeatureIsNotEnabled",
                        "Feature is not enabled: {0}"
                    ),
                    localizedFeatureNames.First()
                ));
            }
        }

        /// <summary>
        /// Checks if a given feature is enabled. Throws <see cref="AbpAuthorizationException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="featureName">Unique feature name</param>
        public static void CheckEnabled(this IFeatureChecker featureChecker, string featureName)
        {
            var localizedFeatureNames = LocalizeFeatureNames(featureChecker, new[] { featureName });

            if (!featureChecker.IsEnabled(featureName))
            {
                throw new AbpAuthorizationException(string.Format(
                    L(
                        featureChecker,
                        "FeatureIsNotEnabled",
                        "Feature is not enabled: {0}"
                    ),
                    localizedFeatureNames.First()
                ));
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

            var localizedFeatureNames = LocalizeFeatureNames(featureChecker, featureNames);

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!(await featureChecker.IsEnabledAsync(featureName)))
                    {
                        throw new AbpAuthorizationException(
                            string.Format(
                                L(
                                    featureChecker,
                                    "AllOfTheseFeaturesMustBeEnabled",
                                    "Required features are not enabled. All of these features must be enabled: {0}"
                                ),
                                string.Join(", ", localizedFeatureNames)
                            )
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
                    string.Format(
                        L(
                            featureChecker,
                            "AtLeastOneOfTheseFeaturesMustBeEnabled",
                            "Required features are not enabled. At least one of these features must be enabled: {0}"
                        ),
                        string.Join(", ", localizedFeatureNames)
                    )
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
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            var localizedFeatureNames = LocalizeFeatureNames(featureChecker, featureNames);

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!featureChecker.IsEnabled(featureName))
                    {
                        throw new AbpAuthorizationException(
                            string.Format(
                                L(
                                    featureChecker,
                                    "AllOfTheseFeaturesMustBeEnabled",
                                    "Required features are not enabled. All of these features must be enabled: {0}"
                                ),
                                string.Join(", ", localizedFeatureNames)
                            )
                        );
                    }
                }
            }
            else
            {
                foreach (var featureName in featureNames)
                {
                    if (featureChecker.IsEnabled(featureName))
                    {
                        return;
                    }
                }

                throw new AbpAuthorizationException(
                    string.Format(
                        L(
                            featureChecker,
                            "AtLeastOneOfTheseFeaturesMustBeEnabled",
                            "Required features are not enabled. At least one of these features must be enabled: {0}"
                        ),
                        string.Join(", ", localizedFeatureNames)
                    )
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
                            string.Format(
                                L(
                                    featureChecker,
                                    "AllOfTheseFeaturesMustBeEnabled",
                                    "Required features are not enabled. All of these features must be enabled: {0}"
                                ),
                                string.Join(", ", featureNames)
                            )
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
                    string.Format(
                        L(
                            featureChecker,
                            "AtLeastOneOfTheseFeaturesMustBeEnabled",
                            "Required features are not enabled. At least one of these features must be enabled: {0}"
                        ),
                        string.Join(", ", featureNames)
                    )
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
        public static void CheckEnabled(this IFeatureChecker featureChecker, int tenantId, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!featureChecker.IsEnabled(tenantId, featureName))
                    {
                        throw new AbpAuthorizationException(
                            string.Format(
                                L(
                                    featureChecker,
                                    "AllOfTheseFeaturesMustBeEnabled",
                                    "Required features are not enabled. All of these features must be enabled: {0}"
                                ),
                                string.Join(", ", featureNames)
                            )
                        );
                    }
                }
            }
            else
            {
                foreach (var featureName in featureNames)
                {
                    if (featureChecker.IsEnabled(tenantId, featureName))
                    {
                        return;
                    }
                }

                throw new AbpAuthorizationException(
                    string.Format(
                        L(
                            featureChecker,
                            "AtLeastOneOfTheseFeaturesMustBeEnabled",
                            "Required features are not enabled. At least one of these features must be enabled: {0}"
                        ),
                        string.Join(", ", featureNames)
                    )
                );
            }
        }
               
        public static string L(IFeatureChecker featureChecker, string name, string defaultValue)
        {
            if (!(featureChecker is IIocManagerAccessor))
            {
                return defaultValue;
            }

            var iocManager = (featureChecker as IIocManagerAccessor).IocManager;
            using (var localizationManager = iocManager.ResolveAsDisposable<ILocalizationManager>())
            {
                return localizationManager.Object.GetString(AbpConsts.LocalizationSourceName, name);
            }
        }

        public static string[] LocalizeFeatureNames(IFeatureChecker featureChecker, string[] featureNames)
        {
            if (!(featureChecker is IIocManagerAccessor))
            {
                return featureNames;
            }

            var iocManager = (featureChecker as IIocManagerAccessor).IocManager;
            using (var localizationContext = iocManager.ResolveAsDisposable<ILocalizationContext>())
            {
                using (var featureManager = iocManager.ResolveAsDisposable<IFeatureManager>())
                {
                    return featureNames.Select(featureName =>
                    {
                        var feature = featureManager.Object.GetOrNull(featureName);
                        return feature?.DisplayName == null
                            ? featureName
                            : feature.DisplayName.Localize(localizationContext.Object);
                    }).ToArray();
                }
            }
        }
    }
}