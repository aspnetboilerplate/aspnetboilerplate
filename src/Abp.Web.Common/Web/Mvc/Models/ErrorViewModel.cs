using System;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Models
{
    public class ErrorViewModel
    {
        public ErrorInfo ErrorInfo { get; set; }

        public Exception Exception { get; set; }

        public ErrorViewModel()
        {
            
        }

        public ErrorViewModel(ErrorInfo errorInfo, Exception exception = null)
        {
            ErrorInfo = errorInfo;
            Exception = exception;
        }
    }
}
