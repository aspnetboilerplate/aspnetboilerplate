using Abp.Domain.Entities;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Standard filters of ABP.
    /// </summary>
    public static class AbpDataFilters
    {
        /// <summary>
        /// Soft delete filter. Filters soft-deleted data.
        /// See <see cref="ISoftDelete"/> interface.
        /// </summary>
        public const string SoftDelete = "SoftDelete";
    }
}