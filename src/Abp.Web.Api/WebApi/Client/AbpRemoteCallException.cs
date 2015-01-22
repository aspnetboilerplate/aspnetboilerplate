using System;
using System.Runtime.Serialization;
using Abp.Web.Models;

namespace Abp.WebApi.Client
{
    /// <summary>
    /// This exception is thrown when a remote method call made and remote application sent an error message.
    /// </summary>
    [Serializable]
    public class AbpRemoteCallException : AbpException
    {
        /// <summary>
        /// Remote error information.
        /// </summary>
        public ErrorInfo ErrorInfo { get; set; }

        /// <summary>
        /// Creates a new <see cref="AbpException"/> object.
        /// </summary>
        public AbpRemoteCallException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="AbpException"/> object.
        /// </summary>
        public AbpRemoteCallException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="AbpException"/> object.
        /// </summary>
        /// <param name="errorInfo">Exception message</param>
        public AbpRemoteCallException(ErrorInfo errorInfo)
            : base(errorInfo.Message)
        {
            ErrorInfo = errorInfo;
        }
    }
}
