using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Abp.Exceptions;

namespace Abp.Runtime.Validation
{
    /// <summary>
    /// This exception type is used to throws validation exceptions.
    /// </summary>
    [Serializable]
    public class AbpValidationException : AbpException
    {
        /// <summary>
        /// Detailed list of validation errors for this exception.
        /// </summary>
        public List<ValidationResult> ValidationErrors { get; set; }

        /// <summary>
        /// Contstructor.
        /// </summary>
        public AbpValidationException()
        {

        }

        /// <summary>
        /// Contstructor for serializing.
        /// </summary>
        public AbpValidationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AbpValidationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AbpValidationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
