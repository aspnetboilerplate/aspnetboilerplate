using System;
using Abp.Dependency;
using Castle.Core.Logging;

namespace Abp.Web.Security.AntiForgery
{
    public class AbpAntiForgeryManager : IAbpAntiForgeryManager, IAbpAntiForgeryValidator, ITransientDependency
    {
        public ILogger Logger { protected get; set; }

        public IAbpAntiForgeryConfiguration Configuration { get; }

        public AbpAntiForgeryManager(IAbpAntiForgeryConfiguration configuration)
        {
            Configuration = configuration;
            Logger = NullLogger.Instance;
        }

        public virtual string GenerateToken()
        {
            return Guid.NewGuid().ToString("D");
        }

        public virtual bool IsValid(string cookieValue, string tokenValue)
        {
            return cookieValue == tokenValue;
        }
    }
}