using System.Reflection;

namespace Abp.Web.Security
{
    public interface ICsrfTokenManager
    {
        ICsrfConfiguration Configuration { get; }

        ICsrfTokenGenerator TokenGenerator { get; }

        bool ShouldValidate(MethodInfo methodInfo, HttpVerb httpVerb, bool defaultValue = false);
    }
}
