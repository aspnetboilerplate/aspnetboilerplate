namespace Abp.Organizations
{
    /// <summary>
    /// This interface is implemented entities those may have an <see cref="OrganizationUnit"/>.
    /// </summary>
    public interface IMayHaveOrganizationUnit
    {
        /// <summary>
        /// <see cref="OrganizationUnit"/>'s Id which this entity belongs to.
        /// Can be null if this entity is not related to any <see cref="OrganizationUnit"/>.
        /// </summary>
        long? OrganizationUnitId { get; set; }
    }
}