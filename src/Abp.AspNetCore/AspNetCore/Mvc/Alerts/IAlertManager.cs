namespace Abp.AspNetCore.Mvc.Alerts
{
    public interface IAlertManager
    {
        AlertList Alerts { get; }
    }
}
