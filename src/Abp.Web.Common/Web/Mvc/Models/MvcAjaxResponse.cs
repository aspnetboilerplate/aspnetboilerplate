using System;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Models
{
    /// <summary>
    /// This class is used to create standard responses for ajax requests.
    /// </summary>
    [Serializable]
    public class MvcAjaxResponse : MvcAjaxResponse<object>
    {
        /// <summary>
        /// Creates an <see cref="MvcAjaxResponse"/> object.
        /// <see cref="AjaxResponse.Success"/> is set as true.
        /// </summary>
        public MvcAjaxResponse()
        {

        }

        /// <summary>
        /// Creates an <see cref="MvcAjaxResponse"/> object with <see cref="AjaxResponse.Success"/> specified.
        /// </summary>
        /// <param name="success">Indicates success status of the result</param>
        public MvcAjaxResponse(bool success)
            : base(success)
        {

        }

        /// <summary>
        /// Creates an <see cref="MvcAjaxResponse"/> object with <see cref="AjaxResponse.Result"/> specified.
        /// <see cref="AjaxResponse.Success"/> is set as true.
        /// </summary>
        /// <param name="result">The actual result object of ajax request</param>
        public MvcAjaxResponse(object result)
            : base(result)
        {

        }

        /// <summary>
        /// Creates an <see cref="MvcAjaxResponse"/> object with <see cref="AjaxResponse.Error"/> specified.
        /// <see cref="AjaxResponse.Success"/> is set as false.
        /// </summary>
        /// <param name="error">Error details</param>
        /// <param name="unAuthorizedRequest">Used to indicate that the current user has no privilege to perform this request</param>
        public MvcAjaxResponse(ErrorInfo error, bool unAuthorizedRequest = false)
            : base(error, unAuthorizedRequest)
        {

        }
    }
}