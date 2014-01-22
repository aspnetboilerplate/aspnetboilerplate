using System;
using System.Runtime.Serialization;
using Abp.Exceptions;

namespace Abp.Application.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AbpAuthorizationException : AbpException
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        public AbpAuthorizationException()
        {

        }

        /// <summary>
        /// Contstructor for serializing.
        /// </summary>
        public AbpAuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AbpAuthorizationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AbpAuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}