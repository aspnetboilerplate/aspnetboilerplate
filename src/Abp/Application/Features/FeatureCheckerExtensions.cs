using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Threading;

namespace Abp.Application.Features
{
    /// <summary>
    /// 
    /// </summary>
    public static class FeatureCheckerExtensions
    {
        public static bool IsEnabled(this IFeatureChecker featureChecker, string name)
        {
            return AsyncHelper.RunSync(() => featureChecker.IsEnabledAsync(name));
        }

        public static bool IsEnabled(this IFeatureChecker featureChecker, int tenantId, string name)
        {
            return AsyncHelper.RunSync(() => featureChecker.IsEnabledAsync(tenantId, name));
        }

        public static string GetValue(this IFeatureChecker featureChecker, int tenantId, string name)
        {
            return AsyncHelper.RunSync(() => featureChecker.GetValueAsync(tenantId, name));
        }

        public static string GetValue(this IFeatureChecker featureChecker, string name)
        {
            return AsyncHelper.RunSync(() => featureChecker.GetValueAsync(name));
        }

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
        
        public static void CheckEnabled(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            AsyncHelper.RunSync(() => featureChecker.CheckEnabledAsync(requiresAll, featureNames));
        }

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
            else
            {
                foreach (var featureName in featureNames)
                {
                    if (await featureChecker.IsEnabledAsync(featureName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static bool IsEnabled(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            return AsyncHelper.RunSync(() => featureChecker.IsEnabledAsync(requiresAll, featureNames));
        }
    }
}