using System;

namespace Abp.Web.Models
{
    /// <summary>
    /// Used to store informations about an error.
    /// </summary>
    [Serializable]
    public class ErrorInfo
    {
        private static IExceptionToErrorInfoConverter _converter = new DefaultExceptionToErrorInfoConverter();

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
        /// Creates a new instance of <see cref="ErrorInfo"/> using given exception object.
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Created <see cref="ErrorInfo"/> object</returns>
        public static ErrorInfo ForException(Exception exception)
        {
            return _converter.Convert(exception);
        }

        /// <summary>
        /// Adds an exception converter that is used by <see cref="ForException"/> method.
        /// </summary>
        /// <param name="converter">Converter object</param>
        public static void AddExceptionConverter(IExceptionToErrorInfoConverter converter)
        {
            converter.Next = _converter;
            _converter = converter;
        }
    }
}