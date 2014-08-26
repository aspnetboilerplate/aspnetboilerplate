using System;

namespace Abp.Web.Models
{
    /// <summary>
    /// Used to store informations about an error.
    /// </summary>
    public class AbpErrorInfo //TODO: Rename to ErrorInfo?
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
        /// Creates a new instance of <see cref="AbpErrorInfo"/>.
        /// </summary>
        public AbpErrorInfo()
        {
            
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbpErrorInfo"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        public AbpErrorInfo(string message)
            : this(message, "")
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="AbpErrorInfo"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public AbpErrorInfo(string message, string details)
        {
            Message = message;
            Details = details;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbpErrorInfo"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public AbpErrorInfo(int code, string message)
            : this(message)
        {
            Code = code;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbpErrorInfo"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public AbpErrorInfo(int code, string message, string details)
            : this(message, details)
        {
            Code = code;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbpErrorInfo"/> using given exception object.
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Created <see cref="AbpErrorInfo"/> object</returns>
        public static AbpErrorInfo ForException(Exception exception)
        {
            return _converter.Convert(exception);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="converter"></param>
        public static void AddExceptionConverter(IExceptionToErrorInfoConverter converter)
        {
            converter.Next = _converter;
            _converter = converter;
        }
    }
}