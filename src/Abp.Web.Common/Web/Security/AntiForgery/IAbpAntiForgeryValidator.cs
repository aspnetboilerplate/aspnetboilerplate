namespace Abp.Web.Security.AntiForgery
{
    /// <summary>
    /// This interface is internally used by ABP framework and normally should not be used by applications.
    /// If it's needed, use 
    /// <see cref="IAbpAntiForgeryManager"/> and cast to 
    /// <see cref="IAbpAntiForgeryValidator"/> to use 
    /// <see cref="IsValid"/> method.
    /// </summary>
    public interface IAbpAntiForgeryValidator
    {
        bool IsValid(string cookieValue, string tokenValue);
    }
}