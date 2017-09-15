namespace Abp.Organizations
{
    /// <summary>
    /// This interface is implemented entities those must have an <see cref="OrganizationUnit"/>.
    /// </summary>
    public interface IMustHaveOrganizationUnit
    {
        /// <summary>
        /// <see cref="OrganizationUnit"/>'s Id which this entity belongs to.
        /// </summary>
        long OrganizationUnitId { get; set; }
    }
}