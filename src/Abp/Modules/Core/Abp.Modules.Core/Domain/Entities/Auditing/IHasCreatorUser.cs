using Abp.Security.Users;

namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// An entity can implement this interface if <see cref="CreatorUser"/> of this entity must be stored.
    /// <see cref="CreatorUser"/> is automatically set when saving <see cref="Entity"/> to database.
    /// </summary>
    public interface IHasCreatorUser
    {
        /// <summary>
        /// Creator of this entity.
        /// </summary>
        AbpUser CreatorUser { get; set; } //TODO: Think to change this to "int CreatorUserId"
    }
}