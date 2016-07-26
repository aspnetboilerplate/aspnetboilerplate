using System;
using System.Runtime.Serialization;
using Abp.Logging;

namespace Abp.Authorization
{
    /// <summary>
    /// This exception is thrown on an unauthorized request.
    /// </summary>
    [Serializable]
    public class AbpAuthorizationException : AbpException, IHasLogSeverity
    {
        /// <summary>
        /// Severity of the exception.
        /// Default: Warn.
        /// </summary>
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Creates a new <see cref="AbpAuthorizationException"/> object.
        /// </summary>
        public AbpAuthorizationException()
        {
            Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Creates a new <see cref="AbpAuthorizationException"/> object.
        /// </summary>
        public AbpAuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="AbpAuthorizationException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AbpAuthorizationException(string message)
            : base(message)
        {
            Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Creates a new <see cref="AbpAuthorizationException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AbpAuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Severity = LogSeverity.Warn;
        }
    }
}