// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecificationException.cs" company="">
//   
// </copyright>
// <summary>
//   The specification exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Abp.Domain.Specifications
{
    using System;

    using Abp;

    /// <summary>
    /// The specification exception.
    /// </summary>
    public class SpecificationException : AbpException
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <c>SpecificationException</c> class.
        /// </summary>
        public SpecificationException() : base() { }
        
        /// <summary>
        /// Initializes a new instance of the <c>SpecificationException</c> class with the specified
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SpecificationException(string message) : base(message) { }
        
        /// <summary>
        /// Initializes a new instance of the <c>SpecificationException</c> class with the specified
        /// error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception.</param>
        public SpecificationException(string message, Exception innerException) : base(message, innerException) { }
        
        /// <summary>
        /// Initializes a new instance of the <c>SpecificationException</c> class with the specified
        /// string formatter and the arguments that are used for formatting the message which
        /// describes the error.
        /// </summary>
        /// <param name="format">The string formatter which is used for formatting the error message.</param>
        /// <param name="args">The arguments that are used by the formatter to build the error message.</param>
        public SpecificationException(string format, params object[] args) : base(string.Format(format, args)) { }
        #endregion
    }
}
