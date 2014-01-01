using System;
using Abp.Exceptions;
using Abp.Web.Localization;

namespace Abp.Web.Exceptions
{
    public static class AbpWebExceptionHelper
    {
        public static string GetMessageToShowToUser(Exception exception)
        {
            if (exception is AbpUserFriendlyException)
            {
                return exception.Message;
            }

            //TODO: Validation exceptions ?

            return LocalizedMessages.InternalServerError;
        }
    }
}
