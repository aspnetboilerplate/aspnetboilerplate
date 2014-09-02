using System;
using System.Text;
using System.Web.Configuration;
using Abp.Runtime.Validation;
using Abp.UI;
using Abp.Utils.Extensions.Collections;
using Abp.Web.Localization;

namespace Abp.Web.Models
{
    //TODO@Halil: I did not like constructing ErrorInfo this way. It works wlll but I think we should change it later...
    internal class DefaultExceptionToErrorInfoConverter : IExceptionToErrorInfoConverter
    {
        public IExceptionToErrorInfoConverter Next { set; private get; }

        private static bool SendAllExceptionsToClients
        {
            get
            {
                return string.Equals(WebConfigurationManager.AppSettings["Abp.Web.SendAllExceptionsToClients"], "true", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public ErrorInfo Convert(Exception exception)
        {
            if (SendAllExceptionsToClients)
            {
                return CreateDetailedErrorInfoFromException(exception);
            }

            if (exception is AggregateException && exception.InnerException != null)
            {
                var aggException = exception as AggregateException;
                if (aggException.InnerException is UserFriendlyException || aggException.InnerException is AbpValidationException)
                {
                    exception = aggException.InnerException;
                }
            }

            if (exception is UserFriendlyException)
            {
                var userFriendlyException = exception as UserFriendlyException;
                return new ErrorInfo(userFriendlyException.Message, userFriendlyException.Details);
            }

            if (exception is AbpValidationException)
            {
                return new ErrorInfo(AbpWebLocalizedMessages.ValidationError);
            }

            return new ErrorInfo(AbpWebLocalizedMessages.InternalServerError);
        }

        private static ErrorInfo CreateDetailedErrorInfoFromException(Exception exception)
        {
            var detailBuilder = new StringBuilder();
            AddExceptionToDetails(exception, detailBuilder);
            return new ErrorInfo(exception.Message, detailBuilder.ToString());
        }

        private static void AddExceptionToDetails(Exception exception, StringBuilder detailBuilder)
        {
            detailBuilder.AppendLine(exception.Message);
            detailBuilder.AppendLine(exception.StackTrace ?? "No StackTrace");

            if (exception.InnerException != null)
            {
                AddExceptionToDetails(exception.InnerException, detailBuilder);                
            }

            if (exception is AggregateException)
            {
                var aggException = exception as AggregateException;
                if (aggException.InnerExceptions.IsNullOrEmpty())
                {
                    return;
                }

                foreach (var innerException in aggException.InnerExceptions)
                {
                    AddExceptionToDetails(innerException, detailBuilder);
                }
            }
        }
    }
}