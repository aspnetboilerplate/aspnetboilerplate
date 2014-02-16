using System;
using System.Runtime.Serialization;

namespace Taskever.Exceptions
{
    [Serializable]
    public class TaskeverException : Exception
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        public TaskeverException()
        {

        }

        /// <summary>
        /// Contstructor for serializing.
        /// </summary>
        public TaskeverException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public TaskeverException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public TaskeverException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}