using System;

namespace Abp.Exceptions
{
    /// <summary>
    /// Base exception type for those are thrown by Abp system for Abp specific exceptions.
    /// TODO: Implement serialization.
    /// </summary>
    public class AbpException : Exception
    {
        /// <summary>
        /// Creates a new AbpException.
        /// </summary>
        public AbpException()
        {

        }

        /// <summary>
        /// Creates a new AbpException.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AbpException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new AbpException.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AbpException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
