using System;
using System.Collections.Generic;

namespace Abp.RealTime
{
    /// <summary>
    /// Represents an online client connected to the application.
    /// </summary>
    public interface IOnlineClient
    {
        /// <summary>
        /// Unique connection Id for this client.
        /// </summary>
        string ConnectionId { get; }

        /// <summary>
        /// IP address of this client.
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// Tenant Id.
        /// </summary>
        int? TenantId { get; }

        /// <summary>
        /// User Id.
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// Connection establishment time for this client.
        /// </summary>
        DateTime ConnectTime { get; }

        /// <summary>
        /// Shortcut to set/get <see cref="Properties"/>.
        /// </summary>
        object this[string key] { get; set; }

        /// <summary>
        /// Can be used to add custom properties for this client.
        /// </summary>
        Dictionary<string, object> Properties { get; }
    }
}