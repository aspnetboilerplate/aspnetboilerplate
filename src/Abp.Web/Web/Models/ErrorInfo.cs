using System;
using System.Linq;

namespace Abp.Web.Models
{
    /// <summary>
    /// Used to store information about an error.
    /// </summary>
    [Serializable]
    public class ErrorInfo
    {
        /// <summary>
        /// Error code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error details.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Validation errors if exists.
        /// </summary>
        public ValidationErrorInfo[] ValidationErrors { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ErrorInfo"/>.
        /// </summary>
        public ErrorInfo()
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="ErrorInfo"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        public ErrorInfo(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ErrorInfo"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        public ErrorInfo(int code)
        {
            Code = code;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ErrorInfo"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public ErrorInfo(int code, string message)
            : this(message)
        {
            Code = code;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ErrorInfo"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public ErrorInfo(string message, string details)
            : this(message)
        {
            Details = details;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ErrorInfo"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public ErrorInfo(int code, string message, string details)
            : this(message, details)
        {
            Code = code;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ErrorInfo"/>.
        /// </summary>
        /// <param name="instanceToClone">Instance to clone</param>
        /// <param name="forAjax">Indicates whether the resultant instance is to be returned in AJAX response</param>
        public ErrorInfo(ErrorInfo instanceToClone, bool forAjax = false)
            : this(instanceToClone.Code, instanceToClone.Message, instanceToClone.Details)
        {
            ValidationErrors =
                (instanceToClone.ValidationErrors != null)
                ? instanceToClone.ValidationErrors.Select(ve => new ValidationErrorInfo(ve, forAjax: forAjax)).ToArray()
                : null;
        }
    }
}