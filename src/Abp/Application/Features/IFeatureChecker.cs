using System.Threading.Tasks;
using Abp.Runtime.Session;

namespace Abp.Application.Features
{
    /// <summary>
    /// This interface should be used to get the value of features
    /// </summary>
    public interface IFeatureChecker
    {
        /// <summary>
        /// Gets the value of a feature by its name.
        /// This is a shortcut for <see cref="GetValueAsync(int, string)"/> that uses <see cref="IAbpSession.TenantId"/> as tenantId.
        /// Note: This method should only be used if a TenantId can be obtained from the session.
        /// </summary>
        /// <param name="name">Unique feature name</param>
        /// <returns>Feature's current value</returns>
        Task<string> GetValueAsync(string name);

        /// <summary>
        /// Gets the value of a feature by its name.
        /// This is a shortcut for <see cref="GetValue(int, string)"/> that uses <see cref="IAbpSession.TenantId"/> as tenantId.
        /// Note: This method should only be used if a TenantId can be obtained from the session.
        /// </summary>
        /// <param name="name">Unique feature name</param>
        /// <returns>Feature's current value</returns>
        string GetValue(string name);

        /// <summary>
        /// Gets the value of a feature for a tenant by the feature's name.
        /// </summary>
        /// <param name="tenantId">Tenant's Id</param>
        /// <param name="name">Unique feature name</param>
        /// <returns>Feature's current value</returns>
        Task<string> GetValueAsync(int tenantId, string name);

        /// <summary>
        /// Gets the value of a feature for a tenant by the feature's name.
        /// </summary>
        /// <param name="tenantId">Tenant's Id</param>
        /// <param name="name">Unique feature name</param>
        /// <returns>Feature's current value</returns>
        string GetValue(int tenantId, string name);

        /// <summary>
        /// Checks if a given feature is enabled.
        /// This should be used for boolean-value features.
        /// 
        /// This is a shortcut for <see cref="IsEnabledAsync(int, string)"/> that uses <see cref="IAbpSession.TenantId"/>.
        /// Note: This method should be used only if the TenantId can be obtained from the session.
        /// </summary>
        /// <param name="featureName">Unique feature name</param>
        /// <returns>True, if the current feature's value is "true".</returns>
        Task<bool> IsEnabledAsync(string featureName);

        /// <summary>
        /// Checks if a given feature is enabled.
        /// This should be used for boolean-value features.
        /// 
        /// This is a shortcut for <see cref="IsEnabled(int, string)"/> that uses <see cref="IAbpSession.TenantId"/>.
        /// Note: This method should be used only if the TenantId can be obtained from the session.
        /// </summary>
        /// <param name="featureName">Unique feature name</param>
        /// <returns>True, if the current feature's value is "true".</returns>
        bool IsEnabled(string featureName);

        /// <summary>
        /// Checks if a given feature is enabled.
        /// This should be used for boolean-value features.
        /// </summary>
        /// <param name="tenantId">Tenant's Id</param>
        /// <param name="featureName">Unique feature name</param>
        /// <returns>True, if the current feature's value is "true".</returns>
        Task<bool> IsEnabledAsync(int tenantId, string featureName);

        /// <summary>
        /// Checks if a given feature is enabled.
        /// This should be used for boolean-value features.
        /// </summary>
        /// <param name="tenantId">Tenant's Id</param>
        /// <param name="featureName">Unique feature name</param>
        /// <returns>True, if the current feature's value is "true".</returns>
        bool IsEnabled(int tenantId, string featureName);
    }
}