using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Dependency;

namespace Abp.Web.Mvc.Alerts
{
    public class PageAlertMessageRenderer : IAlertMessageRenderer
    {
        public string DisplayType { get; } = AlertDisplayType.PageAlert;

        public virtual string Render(List<AlertMessage> alertList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var alertMessage in alertList)
            {
                sb.Append($"<div class=\"alert alert-{alertMessage.Type.ToString().ToLowerInvariant()}\" {(alertMessage.Dismissible ? "alert-dismisable" : "")} role=\"alert\">" +
                              $"<h4 class=\"alert-heading\">{alertMessage.Title}" +
                                  $"{(alertMessage.Dismissible ? "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" : "")}" +
                              $"</h4>" +
                                $"<p>{alertMessage.Text}</p>" +
                          $"</div>");
            }
            return sb.ToString();
        }
    }
}
