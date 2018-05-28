using Abp.Dependency;

namespace Abp.Web.Mvc.Alerts
{
    //todo: use ScopedDependency (inherit from IScopedDependency)
    public class AlertManager : IAlertManager, ISingletonDependency
    {
        public AlertList Alerts { get; }

        public AlertManager()
        {
            Alerts = new AlertList();
        }
    }
}