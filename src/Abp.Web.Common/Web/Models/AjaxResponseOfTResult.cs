using System;

namespace Abp.Web.Models
{
    /// <summary>
    /// This class is used to create standard responses for AJAX requests.
    /// </summary>
    [Serializable]
    public class AjaxResponse<TResult>: AjaxResponseBase
    {
        /// <summary>
        /// The actual result object of AJAX request.
        /// It is set if <see cref="AjaxResponseBase.Success"/> is true.
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="Result"/> specified.
        /// <see cref="AjaxResponseBase.Success"/> is set as true.
        /// </summary>
        /// <param name="result">The actual result object of AJAX request</param>
        public AjaxResponse(TResult result)
        {
            Result = result;
            Success = true;
        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object.
        /// <see cref="AjaxResponseBase.Success"/> is set as true.
        /// </summary>
        public AjaxResponse()
        {
            Success = true;
        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="AjaxResponseBase.Success"/> specified.
        /// </summary>
        /// <param name="success">Indicates success status of the result</param>
        public AjaxResponse(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="AjaxResponseBase.Error"/> specified.
        /// <see cref="AjaxResponseBase.Success"/> is set as false.
        /// </summary>
        /// <param name="error">Error details</param>
        /// <param name="unAuthorizedRequest">Used to indicate that the current user has no privilege to perform this request</param>
        public AjaxResponse(ErrorInfo error, bool unAuthorizedRequest = false)
        {
            Error = error;
            UnAuthorizedRequest = unAuthorizedRequest;
            Success = false;
        }
    }
}