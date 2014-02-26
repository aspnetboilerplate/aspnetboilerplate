using Abp.Security.Users;

namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// An entity can implement this interface if <see cref="CreatorUserId"/> of this entity must be stored.
    /// <see cref="CreatorUserId"/> is automatically set when saving <see cref="Entity"/> to database.
    /// </summary>
    public interface IHasCreatorUser
    {
        /// <summary>
        /// Creator of this entity.
        /// </summary>
        int? CreatorUserId { get; set; }
    }
}