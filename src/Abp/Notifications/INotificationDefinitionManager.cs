using System.Collections.Generic;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to manage notification definitions.
    /// </summary>
    public interface INotificationDefinitionManager
    {
        void Add(NotificationDefinition notificationDefinition);

        NotificationDefinition Get(string name);

        NotificationDefinition GetOrNull(string name);

        IReadOnlyList<NotificationDefinition> GetAll();

        //TODO: ...
    }
}