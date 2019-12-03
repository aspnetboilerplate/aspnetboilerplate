### Introduction

Alerts is a common way to show messages to the user after a web request. Examples:

<img src="images/UI-Alerts.png" alt="UI Alerts" class="img-thumbnail" />

ASP.NET Boilerplate provides a simple alert infrastructure for **MVC** applications (both for ASP.NET Core MVC & ASP.NET MVC 5.x).

> UI Alert system is designed for the actions those return **views**. An action returns a JSON/object result can not use the UI alert system (because it's something related to UI rather than APIs).

### Add Alerts

`IAlertManager` can be injected and used to add alerts into its `Alerts` collection. `AbpControllerBase` and `AbpPageModel`  base classes already injects it and have a shortcut to use the `Alerts` collection.

> All alert messages added in the same web request are added in the same collection even if they are added by different classes/controllers/services.

Example MVC Controller action (that produces the output shown in the figure above):

```c#
public class AlertsTestController : AbpControllerBase
{
    public IActionResult Index()
    {
        Alerts.Danger("Danger alert message!", "Test Alert");//AlertDisplayType.PageAlert is default.
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
```

### Show Alerts

MVC based [startup templates](https://aspnetboilerplate.com/Templates) already shows alerts on the page by default. So, nothing to do if you are using one of the startup templates.



##### Adding Alerts to Your Custom Template

Alert messages are served by `IAlertMessageRenderer`s. 

We have two type of alert message renderer by default.

- *PageAlertMessageRenderer*
  It returns bootstrap alert div so that you can add  it in your page.

- *ToastrAlertMessageRenderer*
  It return toastr scripts. That's why it should be located in `scripts` section in your page.

  > Important Note: To show toastr, you should add its css and js reference. (see the example below.)

To show them in your page you can use `IAlertMessageRendererManager`

```html
@using Abp.Web.Mvc.Alerts
@inject IAlertMessageRendererManager AlertMessageRendererManager

@section styles{
    <link href="~/lib/toastr/toastr.css" rel="stylesheet" />
}
<!--...-->
<div id="AbpPageAlerts">
    @Html.Raw(AlertMessageRendererManager.Render(AlertDisplayType.PageAlert))
</div>
<!--...-->

@section scripts{
    <script src="~/lib/toastr/toastr.min.js" type="text/javascript"></script>
    <script src="~/Abp/Framework/scripts/libs/abp.toastr.js" type="text/javascript"></script>

    @Html.Raw(AlertMessageRendererManager.Render(AlertDisplayType.Toastr))
}
```



##### Adding New Alert Renderer

Create new class inherited from `IAlertMessageRenderer`. 

Name your alert renderer type.

(`PageAlert` and `Toastr` are already used by `AlertDisplayType`. Pick another name.).

```c#
    public class MyOwnAlertMessageRenderer : IAlertMessageRenderer
    {
        public string DisplayType { get; } = "MyOwnAlertRenderer";//Name your renderer

        public string Render(List<AlertMessage> alertList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var alertMessage in alertList)
            {
                sb.Append($"<div>{alertMessage.Text}</div>");//your alert template
            }
            return sb.ToString();
        }
    }
```

Then you can use alert with your own renderer.

```c#
 Alerts.Danger("Danger toast message!", "Test Toast", dismissible: false, displayType: "MyOwnAlertRenderer");
```

```c#
 @Html.Raw(AlertMessageRendererManager.Render("MyOwnAlertRenderer"))
```



> If you want to access to the alerts added by the current request, you can always inject the `IAlertManager` and use its `Alerts` collection.