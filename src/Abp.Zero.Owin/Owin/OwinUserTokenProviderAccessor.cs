using Abp.Authorization.Users;
using Castle.Core.Logging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Abp.Owin
{
    public class OwinUserTokenProviderAccessor : IUserTokenProviderAccessor
    {
        public ILogger Logger { get; set; }

        public IDataProtectionProvider DataProtectionProvider { get; set; }

        public OwinUserTokenProviderAccessor()
        {
            Logger = NullLogger.Instance;
        }

        public IUserTokenProvider<TUser, long> GetUserTokenProviderOrNull<TUser>()
            where TUser : AbpUser<TUser>
        {
            if (DataProtectionProvider == null)
            {
                Logger.Debug("DataProtectionProvider has not been set yet.");
                return null;
            }

            return new DataProtectorTokenProvider<TUser, long>(DataProtectionProvider.Create("ASP.NET Identity"));
        }
    }
}