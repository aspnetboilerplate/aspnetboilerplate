namespace Abp.Web.Security
{
    public interface ICsrfTokenManager
    {
        ICsrfConfiguration Configuration { get; }

        ICsrfTokenGenerator TokenGenerator { get; }
    }
}
