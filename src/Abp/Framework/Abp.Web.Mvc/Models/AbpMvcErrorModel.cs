using System;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Models
{
    public class AbpMvcErrorModel
    {
        public AbpErrorInfo ErrorInfo { get; set; }

        public Exception Exception { get; set; }

        public AbpMvcErrorModel()
        {
            
        }

        public AbpMvcErrorModel(Exception exception)
        {
            Exception = exception;
            ErrorInfo = AbpErrorInfo.ForException(exception);
        }
    }
}
