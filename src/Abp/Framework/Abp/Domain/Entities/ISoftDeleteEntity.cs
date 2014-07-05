namespace Abp.Domain.Entities
{
    /// <summary>
    /// Used to standardize soft deleting entities. Soft-delete entities are not actually deleted, but marked as IsDeleted = true in the database.
    /// </summary>
    public interface ISoftDeleteEntity
    {
        /// <summary>
        /// Used to mark an Entity as 'Deleted'. Deleted entities are not deleted in database but can not retrived to the application.
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
