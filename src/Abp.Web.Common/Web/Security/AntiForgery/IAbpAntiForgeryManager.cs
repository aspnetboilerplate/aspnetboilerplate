using System.Reflection;

namespace Abp.Web.Security.AntiForgery
{
    public interface IAbpAntiForgeryManager
    {
        IAbpAntiForgeryConfiguration Configuration { get; }

        string GenerateToken();

        bool IsValid(string cookieValue, string tokenValue);

        bool ShouldValidate(MethodInfo methodInfo, HttpVerb httpVerb, bool defaultValue);
    }
}
