using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;

namespace Abp.Web.Mvc.Alerts
{
    public interface IAlertMessageRendererManager : ITransientDependency
    {
        string Render(string alertDisplayType);
    }
}
