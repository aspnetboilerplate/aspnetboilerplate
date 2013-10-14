using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Models
{
    public class AbpMvcAjaxResult : AbpAjaxResult
    {
        /// <summary>
        /// This property can be used to redirect user to a specified URL.
        /// </summary>
        public string TargetUrl { get; set; }

        public AbpMvcAjaxResult(bool success) : base(success)
        {
        }

        public AbpMvcAjaxResult(object result) : base(result)
        {
        }

        public AbpMvcAjaxResult(AbpErrorInfo error, bool unAuthorizedRequest = false) : base(error, unAuthorizedRequest)
        {
        }
    }
}
