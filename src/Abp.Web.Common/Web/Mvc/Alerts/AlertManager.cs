using Abp.Dependency;

namespace Abp.Web.Mvc.Alerts
{
    //todo: use ScopedDependency instead of transient
    public class AlertManager : IAlertManager, ITransientDependency
    {
        public AlertList Alerts { get; }

        public AlertManager()
        {
            Alerts = new AlertList();
        }
    }
}