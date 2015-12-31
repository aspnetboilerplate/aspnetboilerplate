using System;

namespace Abp.Web.Models
{
    /// <summary>
    /// Used to determine how ABP should wrap response on the web layer.
    /// </summary>
    public class WrapResultAttribute : Attribute
    {
        /// <summary>
        /// Wrap result on success.
        /// Default: true.
        /// </summary>
        public bool OnSuccess { get; set; }

        /// <summary>
        /// Wrap result on error.
        /// Default: true.
        /// </summary>
        public bool OnError { get; set; }

        /// <summary>
        /// Log errors.
        /// Default: true.
        /// </summary>
        public bool LogError { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapResultAttribute"/> class.
        /// </summary>
        /// <param name="onSuccess">Wrap result on success.</param>
        /// <param name="onError">Wrap result on error.</param>
        public WrapResultAttribute(bool onSuccess = true, bool onError = true)
        {
            OnSuccess = onSuccess;
            OnError = onError;

            LogError = true;
        }
    }
}
