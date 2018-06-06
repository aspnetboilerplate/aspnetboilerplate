### Introduction

You can use UI Alerts to show alerts in views.

<img src="images/UI-Alerts.png" alt="UI Alerts" class="img-thumbnail" />

### Example Usage

Add messages to Alerts collection. Alerts can be initialized from any class in same web request except ajax requests.

```c#
public class AlertsTestController : AbpControllerBase
{
    public IActionResult Index()
    {
        Alerts.Danger("Danger alert message!", "Test Alert");
        Alerts.Warning("Warning alert message!", "Test Alert");
        Alerts.Info("Info alert message!", "Test Alert");
        Alerts.Success("Success alert message!", "Test Alert");

        return View();
    }
}
```

And inject AlertManager into the view you want to use.
