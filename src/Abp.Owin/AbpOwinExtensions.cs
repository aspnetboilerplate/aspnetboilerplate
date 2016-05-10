using Abp.Threading;
using Owin;

namespace Abp.Owin
{
    /// <summary>
    ///     OWIN extension methods for ABP.
    /// </summary>
    public static class AbpOwinExtensions
    {
        /// <summary>
        ///     Uses ABP.
        /// </summary>
        public static void UseAbp(this IAppBuilder app)
        {
            ThreadCultureSanitizer.Sanitize();
        }
    }
}