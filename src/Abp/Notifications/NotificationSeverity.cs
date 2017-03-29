using System;

namespace Abp.Notifications
{
    /// <summary>
    /// Notification severity.
    /// </summary>
    public enum NotificationSeverity : byte
    {
        /// <summary>
        /// Info.
        /// </summary>
        Info = 0,
        
        /// <summary>
        /// Success.
        /// </summary>
        Success = 1,
        
        /// <summary>
        /// Warn.
        /// </summary>
        Warn = 2,
        
        /// <summary>
        /// Error.
        /// </summary>
        Error = 3,

        /// <summary>
        /// Fatal.
        /// </summary>
        Fatal = 4
    }
}