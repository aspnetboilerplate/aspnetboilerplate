using System;

namespace Abp.Web.Models
{
    /// <summary>
    /// Used to determine how ABP should wrap response on the web layer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class WrapResultAttribute : Attribute
    {
        /// <summary>
        /// Wrap result on success.
        /// </summary>
        public bool WrapOnSuccess { get; set; }

        /// <summary>
        /// Wrap result on error.
        /// </summary>
        public bool WrapOnError { get; set; }

        /// <summary>
        /// Log errors.
        /// Default: true.
        /// </summary>
        public bool LogError { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapResultAttribute"/> class.
        /// </summary>
        /// <param name="wrapOnSuccess">Wrap result on success.</param>
        /// <param name="wrapOnError">Wrap result on error.</param>
        public WrapResultAttribute(bool wrapOnSuccess = true, bool wrapOnError = true)
        {
            WrapOnSuccess = wrapOnSuccess;
            WrapOnError = wrapOnError;

            LogError = true;

        }
    }
}
