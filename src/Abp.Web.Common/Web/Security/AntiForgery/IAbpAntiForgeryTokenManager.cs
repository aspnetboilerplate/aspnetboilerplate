using System.Reflection;

namespace Abp.Web.Security.AntiForgery
{
    public interface IAbpAntiForgeryTokenManager
    {
        IAbpAntiForgeryConfiguration Configuration { get; }

        IAbpAntiForgeryTokenGenerator TokenGenerator { get; }

        bool ShouldValidate(MethodInfo methodInfo, HttpVerb httpVerb, bool defaultValue = false);
    }
}
