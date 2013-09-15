using System;
using System.Runtime.Serialization;

namespace Abp.Exceptions
{
    /// <summary>
    /// This exception type is directly shown to the user.
    /// </summary>
    [Serializable]
    public class UserFriendlyException : Exception
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        public UserFriendlyException()
        {

        }

        /// <summary>
        /// Contstructor for serializing.
        /// </summary>
        public UserFriendlyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public UserFriendlyException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public UserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}