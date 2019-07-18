using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;

namespace Abp.Web.Mvc.Alerts
{
    public interface IAlertMessageRenderer : ITransientDependency
    {
        string DisplayType { get; }
        string Render(List<AlertMessage> alertList);
    }
}
