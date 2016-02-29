namespace Adorable.Notifications
{
    /// <summary>
    /// Used as a context while defining notifications.
    /// </summary>
    public interface INotificationDefinitionContext
    {
        /// <summary>
        /// Gets the notification definition manager.
        /// </summary>
        INotificationDefinitionManager Manager { get; }
    }
}