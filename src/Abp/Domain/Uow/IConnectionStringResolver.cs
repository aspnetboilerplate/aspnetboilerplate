using Abp.MultiTenancy;

namespace Abp.Domain.Uow
{
    public interface IConnectionStringResolver
    {
        string GetNameOrConnectionString(MultiTenancySides? multiTenancySide = null);
    }
}