namespace Abp.Application.Authorization
{
    /// <summary>
    /// Defines members of a feature based authorization class.
    /// </summary>
    public interface IFeatureBasedAuthorization
    {
        /// <summary>
        /// A list of features to authorize.
        /// A user is authorized if any of the features is allowed.
        /// </summary>
        string[] Features { get; set; }
    }
}