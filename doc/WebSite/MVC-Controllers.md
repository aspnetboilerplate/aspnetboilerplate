### Introduction

ASP.NET Boilerplate is integrated to **ASP.NET MVC Controllers** via
**Abp.Web.Mvc** nuget package. You can create regular MVC Controllers as
you always do. [Dependency
Injection](/Pages/Documents/Dependency-Injection) properly works for
regular MVC Controllers. But you should derive your controllers from
**AbpController**, which provides several benefits and better integrates
to ASP.NET Boilerplate.

### AbpController Base Class

This is a simple controller derived from AbpController:

    public class HomeController : AbpController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
                

#### Localization

AbpController defines **L** method to make
[localization](/Pages/Documents/Localization) easier. Example:

    public class HomeController : AbpController
    {
        public HomeController()
        {
            LocalizationSourceName = "MySourceName";
        }

        public ActionResult Index()
        {
            var helloWorldText = L("HelloWorld");

            return View();
        }
    }

You should set **LocalizationSourceName** to make **L** method working.
You can set it in your own base controller class, to not repeat for each
controller.

#### Others

You can also use pre-injected
[AbpSession](/Pages/Documents/Abp-Session),
[EventBus](/Pages/Documents/EventBus-Domain-Events), [PermissionManager,
PermissionChecker](/Pages/Documents/Authorization),
[SettingManager](/Pages/Documents/Setting-Management), [FeatureManager,
FeatureChecker](/Pages/Documents/Feature-Management),
[LocalizationManager](/Pages/Documents/Localization),
[Logger](/Pages/Documents/Logging),
[CurrentUnitOfWork](/Pages/Documents/Unit-Of-Work) base properties and
more.

### Filters

#### Exception Handling & Result Wrapping

All exceptions are automatically handled, logged and a proper response
is returned to the client. See [exception
handling](/Pages/Documents/Handling-Exceptions) documentation for more.

ASP.NET Boilerplate also **wraps** action results by default if return
type is **JsonResult** (or Task&lt;JsonResult&gt; for async actions).

You can change exception handling and wrapping by using **WrapResult**
and **DontWrapResult** attributes for controllers or actions or from
[startup configuration](Startup-Configuration.html) (using
Configuration.Modules.AbpMvc()...) globally. See [ajax
documentation](/Pages/Documents/Javascript-API/AJAX) for more.

#### Audit Logging

**AbpMvcAuditFilter** is used to integrate to [audit logging
system](Audit-Logging.html). It logs all requests to all actions by
default (if auditing is not disabled). You can control audit logging
using **Audited** and **DisableAuditing** attributes for actions and
controllers.

#### Validation

**AbpMvcValidationFilter** automatically checks **ModelState.IsValid**
and prevents execution of the action if it's not valid. Also, implements
input DTO validation described in the [validation
documentation](Validating-Data-Transfer-Objects.html).

#### Authorization

You can use **AbpMvcAuthorize** attribute for your controllers or
actions to prevent unauthorized users to use your controllers and
actions. Example:

    public class HomeController : AbpController
    {
        [AbpMvcAuthorize("MyPermissionName")]
        public ActionResult Index()
        {
            return View();
        }
    }

You can define **AllowAnonymous** attribute for actions or controllers
to suppress authentication/authorization. AbpController also defines
**IsGranted** method as a shortcut to check permissions.

See [authorization](/Pages/Documents/Authorization) documentation for
more.

#### Unit Of Work

**AbpMvcUowFilter** is used to integrate to [Unit of
Work](Unit-Of-Work.html) system. It automatically begins a new unit of
work before an action execution and completes unit of work after action
exucition (if no exception is thrown).

You can use **UnitOfWork** attribute to control behaviour of UOW for an
action. You can also use startup configuration to change default unit of
work attribute for all actions.

#### Anti Forgery

**AbpAntiForgeryMvcFilter** is used to auto protect MVC actions for
POST, PUT and DELETE requests from CSRF/XSRF attacks. See [CSRF
documentation](XSRF-CSRF-Protection.html) for more.

### Model Binders

**AbpMvcDateTimeBinder** is used to normalize DateTime (and
Nullable&lt;DateTime&gt;) inputs using **Clock.Normalize** method.
