namespace Abp.Web.Security.AntiForgery
{
    public interface IAbpAntiForgeryTokenGenerator
    {
        string Generate();
    }
}