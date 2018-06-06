### Introduction

UI Alerts provide a collection to add alert messages by type which is rendered on the UI.

#### IAlertManager

```c#
public interface IAlertManager
{
    AlertList Alerts { get; }
}
```

The implementation should be registered as scoped (per request).

#### AlertType

```c#
public enum AlertType
{
    Success,
    Danger,
    Warning,
    Info
}
```

Alert types are matching bootstrap alert classes.

### Example Usage

Add messages to Alerts collection:

```c#
public  async Task<IActionResult> OnPostAsync()
{
    var result = await SignInManager.PasswordSignInAsync(
        Input.UserNameOrEmailAddress,
        Input.Password,
        Input.RememberMe,
        true
    );

    if (result.IsLockedOut)
    {
        Alerts.Warning(L["UserLockedOutMessage"]); //A warning message
        return Page();
    }

    if (!result.Succeeded)
    {
        Alerts.Danger(L["InvalidUserNameOrPassword"]); //A danger/error message
        return Page();
    }

    return Redirect(ReturnUrl);
}
```

And inject it into the view you want to use. There is a minor difference between the usage of DI in ASP.NET Core and ASP.NET MVC5 projects. 

#### ASP.NET Core project example:

```html
@using Abp.Web.Mvc.Alerts
@inject IAlertManager AlertManager

@{
    ViewData["Title"] = "Index";
}

<h2>Alerts test page</h2>

@if (AlertManager.Alerts.Any())
{
    <div id="AbpPageAlerts">
        @foreach (var alertMessage in AlertManager.Alerts)
        {
            <div class="alert alert-@alertMessage.Type.ToString().ToLower() @(alertMessage.Dismissible ? "alert-dismisable" : "")" role="alert">
                @if (alertMessage.Dismissible)
                {
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                }
                <strong>@alertMessage.Title</strong> @alertMessage.Text
            </div>
        }
    </div>
}
```

#### ASP.NET MVC5 project example:

```html
@using Abp.Dependency
@using Abp.Web.Mvc.Alerts
@{
    ViewData["Title"] = "Index";
    AlertList alerts = null;
    IocManager.Instance.Using<IAlertManager>(alertManager => alerts = alertManager.Alerts);
}
<h2>Alerts test page</h2>
@if (alerts.Any())
{
    <div id="AbpPageAlerts">
        @foreach (var alertMessage in alerts)
        {
            <div class="alert alert-@alertMessage.Type.ToString().ToLower() @(alertMessage.Dismissible ? "alert-dismisable" : "")" role="alert">
                @if (alertMessage.Dismissible)
                {
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                }
                <strong>@alertMessage.Title</strong> @alertMessage.Text
            </div>
        }
    </div>
}
```

