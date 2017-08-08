using System.Security.Claims;
using System.Threading;
using Abp.Dependency;

namespace Abp.Runtime.Session
{
    public class DefaultPrincipalAccessor : IPrincipalAccessor, ISingletonDependency
    {
        public virtual ClaimsPrincipal Principal =>
#if NET46
            Thread.CurrentPrincipal as ClaimsPrincipal;
#else
            null;
#endif

        public static DefaultPrincipalAccessor Instance => new DefaultPrincipalAccessor();
    }
}