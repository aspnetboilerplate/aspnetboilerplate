using System.Collections.Generic;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to manage notification definitions.
    /// </summary>
    public interface INotificationDefinitionManager
    {
        void Initialize();

        IReadOnlyList<NotificationDefinition> Get(string name);

        IReadOnlyList<NotificationDefinition> GetOrNull(string name);

        IReadOnlyList<NotificationDefinition> GetAll();

        //TODO: ...
    }
}