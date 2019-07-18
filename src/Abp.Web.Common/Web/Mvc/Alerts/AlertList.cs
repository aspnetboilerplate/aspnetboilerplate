using System.Collections.Generic;

namespace Abp.Web.Mvc.Alerts
{
    public class AlertList : List<AlertMessage>
    {
        public void Add(AlertType type, string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(type, text, title, dismissible, displayType));
        }

        public void Info(string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(AlertType.Info, text, title, dismissible, displayType));
        }

        public void Warning(string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(AlertType.Warning, text, title, dismissible, displayType));
        }

        public void Danger(string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(AlertType.Danger, text, title, dismissible, displayType));
        }

        public void Success(string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(AlertType.Success, text, title, dismissible, displayType));
        }
    }
}