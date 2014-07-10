namespace Abp.Application.Session
{
    public interface IAbpSession
    {
        int? UserId { get; }

        int? TenantId { get; }
    }
}
