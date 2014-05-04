namespace Abp.Domain.Entities
{
    ///// <summary>
    ///// This interface can be implemented by Entities those must be marked as Deleted instead of actually Deleting entity in the database.
    ///// Repositories implement logic to set <see cref="IsDeleted"/> in Delete method and filter entities with <see cref="IsDeleted"/> in GetAll method.
    ///// </summary>
    /// <summary>
    /// Used to standardize soft deleting entities. Soft-delete entities are not actually deleted, but marked as IsDeleted = true.
    /// </summary>
    public interface ISoftDeleteEntity
    {
        /// <summary>
        /// Used to mark an Entity as 'Deleted'. Deleted entities are not deleted in database but can not retrived to the application.
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
