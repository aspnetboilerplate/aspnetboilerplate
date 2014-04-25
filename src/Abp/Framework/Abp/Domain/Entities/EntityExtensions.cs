namespace Abp.Domain.Entities
{
    /// <summary>
    /// Some usefull extension methods for Entities.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Check if this Entity is null of marked as deleted.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsNullOrDeleted(this ISoftDeleteEntity entity)
        {
            return entity == null || entity.IsDeleted;
        }
    }
}