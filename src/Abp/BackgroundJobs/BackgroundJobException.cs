using System;
using System.Runtime.Serialization;

namespace Abp.BackgroundJobs
{
    [Serializable]
    public class BackgroundJobException : AbpException
    {
        public BackgroundJobInfo BackgroundJob { get; set; }

        /// <summary>
        /// Creates a new <see cref="BackgroundJobException"/> object.
        /// </summary>
        public BackgroundJobException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="BackgroundJobException"/> object.
        /// </summary>
        public BackgroundJobException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="BackgroundJobException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public BackgroundJobException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
