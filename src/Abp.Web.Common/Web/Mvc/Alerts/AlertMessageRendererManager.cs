using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abp.Web.Mvc.Alerts
{
    public class AlertMessageRendererManager : IAlertMessageRendererManager
    {
        private readonly IEnumerable<IAlertMessageRenderer> _alertMessageRenderers;
        private readonly IAlertManager _alertManager;

        public AlertMessageRendererManager(IEnumerable<IAlertMessageRenderer> alertMessageRenderers, IAlertManager AlertManager)
        {
            _alertMessageRenderers = alertMessageRenderers;
            _alertManager = AlertManager;
        }

        public string Render(string alertDisplayType)
        {
            if (!_alertManager.Alerts.Any(a => a.DisplayType == alertDisplayType))
            {
                return "";
            }

            var alertMessageRenderer = _alertMessageRenderers.FirstOrDefault(x => x.DisplayType == alertDisplayType);

            return alertMessageRenderer?.Render(_alertManager.Alerts.Where(a => a.DisplayType == alertDisplayType).ToList());
        }
    }
}
