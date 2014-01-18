using System;
using Abp.Exceptions;
using Abp.Web.Localization;

namespace Abp.Web.Models
{
    /// <summary>
    /// Used to store informations about an error.
    /// </summary>
    public class AbpErrorInfo
    {
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
            : this("", message)
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
        /// Creates a new instance of <see cref="AbpErrorInfo"/> using given exception object.
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Created <see cref="AbpErrorInfo"/> object</returns>
        public static AbpErrorInfo ForException(Exception exception)
        {
            if (exception is AbpUserFriendlyException)
            {
                var userFriendlyException = exception as AbpUserFriendlyException;
                return new AbpErrorInfo(userFriendlyException.Message, userFriendlyException.Details);
            }

            //TODO: How to show validation exceptions...?

            return new AbpErrorInfo(AbpWebLocalizedMessages.InternalServerError);
        }
    }
}