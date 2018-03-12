using System;
using System.Collections.Generic;

namespace Abp.EntityHistory
{
    /// <summary>
    /// Used to configure entity history.
    /// </summary>
    public interface IEntityHistoryConfiguration
    {
        /// <summary>
        /// Used to enable/disable entity history system.
        /// Default: true. Set false to completely disable it.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Set true to enable saving entity history if current user is not logged in.
        /// Default: false.
        /// </summary>
        bool IsEnabledForAnonymousUsers { get; set; }

        /// <summary>
        /// List of selectors to select classes/interfaces which should be tracked as default.
        /// </summary>
        IEntityHistorySelectorList Selectors { get; }

        /// <summary>
        /// Ignored types for serialization on entity history tracking.
        /// </summary>
        List<Type> IgnoredTypes { get; }
    }
}
