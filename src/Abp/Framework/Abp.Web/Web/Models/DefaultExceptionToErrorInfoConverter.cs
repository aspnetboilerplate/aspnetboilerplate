using System;
using Abp.Runtime.Validation;
using Abp.UI;
using Abp.Web.Localization;

namespace Abp.Web.Models
{
    internal class DefaultExceptionToErrorInfoConverter : IExceptionToErrorInfoConverter
    {
        public IExceptionToErrorInfoConverter Next { set; private get; }

        public AbpErrorInfo Convert(Exception exception)
        {
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
                return new AbpErrorInfo(userFriendlyException.Message, userFriendlyException.Details);
            }

            if (exception is AbpValidationException)
            {
                return new AbpErrorInfo(AbpWebLocalizedMessages.ValidationError);
            }

            return new AbpErrorInfo(AbpWebLocalizedMessages.InternalServerError);
        }
    }
}