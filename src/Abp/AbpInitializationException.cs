using System;
using System.Runtime.Serialization;

namespace Abp
{
    /// <summary>
    /// This exception is thrown if a problem on ABP initialization progress.
    /// </summary>
    [Serializable]
    public class AbpInitializationException : AbpException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpInitializationException()
        {

        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public AbpInitializationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AbpInitializationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AbpInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
