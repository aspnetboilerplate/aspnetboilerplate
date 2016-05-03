using Abp.MultiTenancy;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Used to get connection string when a database connection is needed.
    /// </summary>
    public interface IConnectionStringResolver
    {
        /// <summary>
        /// Gets a connection string name (in config file) or a valid connection string.
        /// </summary>
        /// <param name="multiTenancySide">The multi tenancy side.</param>
        string GetNameOrConnectionString(MultiTenancySides? multiTenancySide = null);
    }
}