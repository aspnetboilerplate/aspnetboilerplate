using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Reflection;
using Castle.Core.Logging;

namespace Abp.Web.Security.AntiForgery
{
    public class AbpAntiForgeryManager : IAbpAntiForgeryManager, ITransientDependency
    {
        public ILogger Logger { protected get; set; }

        public IAbpAntiForgeryConfiguration Configuration { get; }

        public AbpAntiForgeryManager(IAbpAntiForgeryConfiguration configuration)
        {
            Configuration = configuration;
            Logger = NullLogger.Instance;
        }

        public virtual bool ShouldValidate(MethodInfo methodInfo, HttpVerb httpVerb, bool defaultValue)
        {
            if (!Configuration.IsEnabled)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(ValidateAbpAntiForgeryTokenAttribute), true))
            {
                return true;
            }

            if (Configuration.IgnoredHttpVerbs.Contains(httpVerb))
            {
                return false;
            }

            if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableAbpAntiForgeryTokenValidationAttribute>(methodInfo) != null)
            {
                return false;
            }

            return defaultValue;
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