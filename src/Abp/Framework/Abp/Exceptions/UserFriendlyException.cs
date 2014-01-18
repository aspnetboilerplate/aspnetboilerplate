using System;
using System.Runtime.Serialization;

namespace Abp.Exceptions
{
    /// <summary>
    /// This exception type is directly shown to the user.
    /// </summary>
    [Serializable]
    public class AbpUserFriendlyException : AbpException
    {
        /// <summary>
        /// Additional information about the exception.
        /// </summary>
        public string Details { get; private set; }

        /// <summary>
        /// Contstructor.
        /// </summary>
        public AbpUserFriendlyException()
        {

        }

        /// <summary>
        /// Contstructor for serializing.
        /// </summary>
        public AbpUserFriendlyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AbpUserFriendlyException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="details">Additional information about the exception</param>
        public AbpUserFriendlyException(string message, string details)
            : base(message)
        {
            Details = details;
        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AbpUserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="details">Additional information about the exception</param>
        /// <param name="innerException">Inner exception</param>
        public AbpUserFriendlyException(string message, string details, Exception innerException)
            : base(message, innerException)
        {
            Details = details;
        }
    }
}