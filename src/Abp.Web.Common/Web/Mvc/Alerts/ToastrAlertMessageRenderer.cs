using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abp.Web.Mvc.Alerts
{
    public class ToastrAlertMessageRenderer : IAlertMessageRenderer
    {
        public string DisplayType { get; } = AlertDisplayType.Toastr;

        public virtual string Render(List<AlertMessage> alertList)
        {
            StringBuilder sb = new StringBuilder("<script type=\"text/javascript\">");
            foreach (var alertMessage in alertList)
            {
                sb.Append(
                    string.Format("abp.notify.{0}('{1}','{2}', {{ 'closeButton':  {3} }});",
                        alertMessage.Type.ToString().ToLowerInvariant()
                            .Replace("warning", "warn")
                            .Replace("danger", "error"),
                        alertMessage.Text,
                        alertMessage.Title,
                        alertMessage.Dismissible.ToString().ToLower()));
            }
            sb.Append("</script>");
            return sb.ToString();
        }
    }
}
