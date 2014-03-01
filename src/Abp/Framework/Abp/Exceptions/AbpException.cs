using System;
using System.Runtime.Serialization;

namespace Abp.Exceptions
{
    /// <summary>
    /// Base exception type for those are thrown by Abp system for Abp specific exceptions.
    /// TODO: Move to Abp namespace
    /// </summary>
    [Serializable]
    public class AbpException : Exception
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        public AbpException()
        {

        }

        /// <summary>
        /// Contstructor for serializing.
        /// </summary>
        public AbpException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AbpException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AbpException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
