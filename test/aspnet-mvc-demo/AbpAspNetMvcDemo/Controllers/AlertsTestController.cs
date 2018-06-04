using System.Web.Mvc;

namespace AbpAspNetMvcDemo.Controllers
{
    public class AlertsTestController : DemoControllerBase
    {
        public ActionResult Index()
        {
            Alerts.Danger("Danger alert message!", "Test Alert");
            Alerts.Warning("Warning alert message!", "Test Alert");
            Alerts.Info("Info alert message!", "Test Alert");
            Alerts.Success("Success alert message!", "Test Alert");

            return View();
        }
    }
}