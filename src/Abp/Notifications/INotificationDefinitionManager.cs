//using System.Collections.Generic;

//namespace Abp.Notifications
//{
//    /// <summary>
//    /// Used to manage notification definitions.
//    /// </summary>
//    public interface INotificationDefinitionManager
//    {
//        /// <summary>
//        /// Adds the specified notification definition.
//        /// </summary>
//        /// <param name="notificationDefinition">The notification definition.</param>
//        void Add(NotificationDefinition notificationDefinition);

//        /// <summary>
//        /// Gets a <see cref="NotificationDefinition"/> by it's unique name.
//        /// </summary>
//        NotificationDefinition Get(string name);

//        /// <summary>
//        /// Gets a <see cref="NotificationDefinition"/> by it's unique name.
//        /// Returns null if not found.
//        /// </summary>
//        NotificationDefinition GetOrNull(string name);

//        /// <summary>
//        /// Gets all notification definitions.
//        /// </summary>
//        IReadOnlyList<NotificationDefinition> GetAll();
//    }
//}