using Abp.Domain.Entities;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Standard filters of ABP.
    /// </summary>
    public static class AbpDataFilters
    {
        /// <summary>
        /// "SoftDelete".
        /// Soft delete filter.
        /// Prevents getting deleted data from database.
        /// See <see cref="ISoftDelete"/> interface.
        /// </summary>
        public const string SoftDelete = "SoftDelete";

        /// <summary>
        /// "MustHaveTenant".
        /// Tenant filter to prevent getting data that is
        /// not belong to current tenant.
        /// </summary>
        public const string MustHaveTenant = "MustHaveTenant";

        /// <summary>
        /// "MayHaveTenant".
        /// Tenant filter to prevent getting data that is
        /// not belong to current tenant.
        /// </summary>
        public const string MayHaveTenant = "MayHaveTenant";

        public const string MustHaveBranch = "MustHaveBranch";


        public const string MayHaveBranch = "MayHaveBranch";

        /// <summary>
        /// Standard parameters of ABP.
        /// </summary>
        public static class Parameters
        {
            /// <summary>
            /// "tenantId".
            /// </summary>
            public const string TenantId = "tenantId";
            public const string BranchId = "branchId";

            /// <summary>
            /// "isDeleted".
            /// </summary>
            public const string IsDeleted = "isDeleted";
        }
    }
}