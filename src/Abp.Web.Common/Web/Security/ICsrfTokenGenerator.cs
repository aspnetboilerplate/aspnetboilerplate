namespace Abp.Web.Security
{
    public interface ICsrfTokenGenerator
    {
        string Generate();
    }
}