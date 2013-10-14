using Abp.Web.Models;

namespace Abp.Web.Mvc.Models
{
    /// <summary>
    /// This class is used to create standard responses for ajax requests.
    /// </summary>
    public class AbpMvcAjaxResponse : AbpAjaxResponse
    {
        /// <summary>
        /// This property can be used to redirect user to a specified URL.
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// Creates an <see cref="AbpMvcAjaxResponse"/> object.
        /// <see cref="AbpAjaxResponse.Success"/> is set as true.
        /// </summary>
        public AbpMvcAjaxResponse()
        {

        }

        /// <summary>
        /// Creates an <see cref="AbpMvcAjaxResponse"/> object with <see cref="AbpAjaxResponse.Success"/> specified.
        /// </summary>
        /// <param name="success">Indicates success status of the result</param>
        public AbpMvcAjaxResponse(bool success)
            : base(success)
        {

        }

        /// <summary>
        /// Creates an <see cref="AbpMvcAjaxResponse"/> object with <see cref="AbpAjaxResponse.Result"/> specified.
        /// <see cref="AbpAjaxResponse.Success"/> is set as true.
        /// </summary>
        /// <param name="result">The actual result object of ajax request</param>
        public AbpMvcAjaxResponse(object result)
            : base(result)
        {

        }

        /// <summary>
        /// Creates an <see cref="AbpMvcAjaxResponse"/> object with <see cref="AbpAjaxResponse.Error"/> specified.
        /// <see cref="AbpAjaxResponse.Success"/> is set as false.
        /// </summary>
        /// <param name="error">Error details</param>
        /// <param name="unAuthorizedRequest">Used to indicate that the current user has no privilege to perform this request</param>
        public AbpMvcAjaxResponse(AbpErrorInfo error, bool unAuthorizedRequest = false)
            : base(error, unAuthorizedRequest)
        {

        }
    }
}
