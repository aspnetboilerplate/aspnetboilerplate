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
        /// <param name="innerException">Inner exception</param>
        public AbpUserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}