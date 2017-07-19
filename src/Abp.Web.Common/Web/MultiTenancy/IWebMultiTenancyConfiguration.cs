namespace Abp.Web.MultiTenancy
{
    public interface IWebMultiTenancyConfiguration
    {
        string DomainFormat { get; set; }
    }
}