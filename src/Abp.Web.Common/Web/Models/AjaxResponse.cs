using System;

namespace Abp.Web.Models
{
    /// <summary>
    /// This class is used to create standard responses for ajax requests.
    /// </summary>
    [Serializable]
    public class AjaxResponse : AjaxResponse<object>
    {
        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object.
        /// <see cref="AjaxResponse{TResult}.Success"/> is set as true.
        /// </summary>
        public AjaxResponse()
        {

        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="AjaxResponse{TResult}.Success"/> specified.
        /// </summary>
        /// <param name="success">Indicates success status of the result</param>
        public AjaxResponse(bool success)
            : base(success)
        {

        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="AjaxResponse{TResult}.Result"/> specified.
        /// <see cref="AjaxResponse{TResult}.Success"/> is set as true.
        /// </summary>
        /// <param name="result">The actual result object of ajax request</param>
        public AjaxResponse(object result)
            : base(result)
        {

        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="AjaxResponse{TResult}.Error"/> specified.
        /// <see cref="AjaxResponse{TResult}.Success"/> is set as false.
        /// </summary>
        /// <param name="error">Error details</param>
        /// <param name="unAuthorizedRequest">Used to indicate that the current user has no privilege to perform this request</param>
        public AjaxResponse(ErrorInfo error, bool unAuthorizedRequest = false)
            : base(error, unAuthorizedRequest)
        {

        }
    }
}