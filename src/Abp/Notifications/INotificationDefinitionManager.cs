using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adorable.Notifications
{
    /// <summary>
    /// Used to manage notification definitions.
    /// </summary>
    public interface INotificationDefinitionManager
    {
        /// <summary>
        /// Adds the specified notification definition.
        /// </summary>
        void Add(NotificationDefinition notificationDefinition);

        /// <summary>
        /// Gets a notification definition by name.
        /// Throws exception if there is no notification definition with given name.
        /// </summary>
        NotificationDefinition Get(string name);

        /// <summary>
        /// Gets a notification definition by name.
        /// Returns null if there is no notification definition with given name.
        /// </summary>
        NotificationDefinition GetOrNull(string name);

        /// <summary>
        /// Gets all notification definitions.
        /// </summary>
        IReadOnlyList<NotificationDefinition> GetAll();

        /// <summary>
        /// Checks if given notification (<see cref="name"/>) is available for given user and given tenant.
        /// </summary>
        Task<bool> IsAvailableAsync(string name, int? tenantId, long userId);

        /// <summary>
        /// Gets all available notification definitions for given <see cref="tenantId"/> and <see cref="userId"/>.
        /// </summary>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        Task<IReadOnlyList<NotificationDefinition>> GetAllAvailableAsync(int? tenantId, long userId);
    }
}