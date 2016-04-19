namespace Abp.Domain.Uow
{
    public interface IConnectionStringResolver
    {
        string GetNameOrConnectionString();
    }
}