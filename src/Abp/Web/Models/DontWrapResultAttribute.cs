namespace Abp.Web.Models
{
    /// <summary>
    /// A shortcut for <see cref="WrapResultAttribute"/> to disable wrapping by default.
    /// It sets false to <see cref="WrapResultAttribute.OnSuccess"/> and <see cref="WrapResultAttribute.OnError"/>  properties.
    /// </summary>
    public class DontWrapResultAttribute : WrapResultAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DontWrapResultAttribute"/> class.
        /// </summary>
        public DontWrapResultAttribute()
            : base(false, false)
        {

        }
    }
}