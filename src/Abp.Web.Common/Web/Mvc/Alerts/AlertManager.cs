using Abp.Dependency;

namespace Abp.Web.Mvc.Alerts
{
    public class AlertManager : IAlertManager, IScopedDependency
    {
        public AlertList Alerts { get; }

        public AlertManager()
        {
            Alerts = new AlertList();
        }
    }
}