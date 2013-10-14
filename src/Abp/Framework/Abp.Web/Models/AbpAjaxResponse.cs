namespace Abp.Web.Models
{
    /// <summary>
    /// This class is used to create standard responses for ajax requests.
    /// </summary>
    public class AbpAjaxResponse
    {
        /// <summary>
        /// The actual result object of ajax request.
        /// It must be set if <see cref="Success"/> is true.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Indicates success status of the result.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error details (if <see cref="Success"/> is false.
        /// </summary>
        public AbpErrorInfo Error { get; set; }

        /// <summary>
        /// This property can be used to indicate that the current user has no privilege to perform this request.
        /// </summary>
        public bool UnAuthorizedRequest { get; set; }

        /// <summary>
        /// Creates an <see cref="AbpAjaxResponse"/> object.
        /// <see cref="Success"/> is set as true.
        /// </summary>
        public AbpAjaxResponse()
        {
            Success = true;
        }

        /// <summary>
        /// Creates an <see cref="AbpAjaxResponse"/> object with <see cref="Success"/> specified.
        /// </summary>
        /// <param name="success">Indicates success status of the result</param>
        public AbpAjaxResponse(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// Creates an <see cref="AbpAjaxResponse"/> object with <see cref="Result"/> specified.
        /// <see cref="Success"/> is set as true.
        /// </summary>
        /// <param name="result">The actual result object of ajax request</param>
        public AbpAjaxResponse(object result)
        {
            Result = result;
            Success = true;
        }

        /// <summary>
        /// Creates an <see cref="AbpAjaxResponse"/> object with <see cref="Error"/> specified.
        /// <see cref="Success"/> is set as false.
        /// </summary>
        /// <param name="error">Error details</param>
        /// <param name="unAuthorizedRequest">Used to indicate that the current user has no privilege to perform this request</param>
        public AbpAjaxResponse(AbpErrorInfo error, bool unAuthorizedRequest = false)
        {
            Error = error;
            UnAuthorizedRequest = unAuthorizedRequest;
            Success = false;
        }
    }
}