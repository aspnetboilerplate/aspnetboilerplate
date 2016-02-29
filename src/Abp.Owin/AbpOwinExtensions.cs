using Adorable.Threading;
using Owin;

namespace Adorable.Owin
{
    /// <summary>
    /// OWIN extension methods for ABP.
    /// </summary>
    public static class AbpOwinExtensions
    {
        /// <summary>
        /// Uses ABP.
        /// </summary>
        public static void UseAbp(this IAppBuilder app)
        {
            ThreadCultureSanitizer.Sanitize();
        }
    }
}