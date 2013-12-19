using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taskever.Web.Models.Account
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }

        public string ResetCode { get; set; }
    }
}