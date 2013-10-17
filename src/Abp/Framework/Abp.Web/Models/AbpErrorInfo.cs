using System;
using Abp.Localization;
using Abp.Web.Exceptions;

namespace Abp.Web.Models
{
    public class AbpErrorInfo
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public AbpErrorInfo(string message)
            : this("", message)
        {

        }

        public AbpErrorInfo(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public static AbpErrorInfo ForException(Exception exception)
        {
            return new AbpErrorInfo(AbpWebExceptionHelper.GetMessageToShowToUser(exception));
        }
    }
}