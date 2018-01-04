using System;

namespace Abp.EntityHistory
{
    /// <summary>
    /// Defines some session information that can be useful for applications.
    /// </summary>
    public interface IEntityChangeSetReasonProvider
    {
        /// <summary>
        /// Gets current Reason or null.
        /// </summary>
        string Reason { get; }

        /// <summary>
        /// Used to change <see cref="Reason"/> for a limited scope.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        IDisposable Use(string reason);
    }
}
