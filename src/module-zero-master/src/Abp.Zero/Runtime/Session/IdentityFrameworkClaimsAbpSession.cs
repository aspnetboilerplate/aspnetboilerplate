using System;
using System.Threading;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Microsoft.AspNet.Identity;

namespace Abp.Runtime.Session
{
    /// <summary>
    /// Implements IAbpSession to get session informations from ASP.NET Identity framework.
    /// </summary>
    public class IdentityFrameworkClaimsAbpSession : ClaimsAbpSession, ISingletonDependency
    {
        public override long? UserId
        {
            get
            {
                var userId = Thread.CurrentPrincipal.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                return Convert.ToInt64(userId);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public IdentityFrameworkClaimsAbpSession(IMultiTenancyConfig multiTenancy) 
            : base(multiTenancy)
        {
        }
    }
}