using System.Reflection;
using Abp.Dependency;
using Abp.Reflection;

namespace Abp.Web.Security.AntiForgery
{
    public class AbpAntiForgeryTokenManager : IAbpAntiForgeryTokenManager, ITransientDependency
    {
        public IAbpAntiForgeryConfiguration Configuration { get; }

        public IAbpAntiForgeryTokenGenerator TokenGenerator { get; }
        
        public AbpAntiForgeryTokenManager(
            IAbpAntiForgeryConfiguration configuration, 
            IAbpAntiForgeryTokenGenerator tokenGenerator)
        {
            Configuration = configuration;
            TokenGenerator = tokenGenerator;
        }

        public bool ShouldValidate(MethodInfo methodInfo, HttpVerb httpVerb, bool defaultValue = false)
        {
            if (!Configuration.IsEnabled)
            {
                return false;
            }

            if (!methodInfo.IsDefined(typeof(ValidateAbpAntiForgeryTokenAttribute), true))
            {
                if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableAbpAntiForgeryTokenValidationAttribute>(methodInfo) != null)
                {
                    return false;
                }

                if (Configuration.IgnoredHttpVerbs.Contains(httpVerb))
                {
                    return false;
                }
            }

            return defaultValue;
        }
    }
}