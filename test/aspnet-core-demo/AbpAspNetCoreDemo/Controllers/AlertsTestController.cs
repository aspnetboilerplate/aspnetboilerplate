using Abp.Web.Mvc.Alerts;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class AlertsTestController : DemoControllerBase
    {
        public IActionResult Index()
        {
            Alerts.Danger("Danger alert message!", "Test Alert");
            Alerts.Warning("Warning alert message!", "Test Alert");
            Alerts.Info("Info alert message!", "Test Alert");
            Alerts.Success("Success alert message!", "Test Alert");

            Alerts.Danger("Danger toast message!", "Test Toast", displayType: AlertDisplayType.Toastr);
            Alerts.Warning("Warning toast message!", "Test Toast", displayType: AlertDisplayType.Toastr);
            Alerts.Info("Info toast message!", "Test Toast", displayType: AlertDisplayType.Toastr);
            Alerts.Success("Success toast message!", "Test Toast", displayType: AlertDisplayType.Toastr);

            Alerts.Danger("Danger toast message!", "Test Toast", dismissible: false, displayType: AlertDisplayType.Toastr);
            Alerts.Warning("Warning toast message!", "Test Toast", dismissible: false, displayType: AlertDisplayType.Toastr);
            Alerts.Info("Info toast message!", "Test Toast", dismissible: false, displayType: AlertDisplayType.Toastr);
            Alerts.Success("Success toast message!", "Test Toast", dismissible: false, displayType: AlertDisplayType.Toastr);

            return View();
        }
    }
}