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
        public static bool IsNullOrDeleted(this ISoftDelete entity)
        {
            return entity == null || entity.IsDeleted;
        }
    }
}