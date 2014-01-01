using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Abp.Exceptions;

namespace Abp.Application.Services.Dto.Validation
{
    /// <summary>
    /// This exception type is directly shown to the user.
    /// </summary>
    [Serializable]
    public class AbpValidationException : AbpException
    {
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
