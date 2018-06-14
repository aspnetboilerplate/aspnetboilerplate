### Introduction

ASP.NET Boilerplate is integrated in to the **ASP.NET MVC Controllers** via
the **Abp.Web.Mvc** NuGet package. You can create regular MVC Controllers as
you always do. [Dependency
Injection](/Pages/Documents/Dependency-Injection) properly works for
regular MVC Controllers, but you should derive your controllers from
**AbpController**, which provides several benefits and provides for better integration
in to ASP.NET Boilerplate.

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

You should set **LocalizationSourceName** to make the **L** method work.
You can set it in your own base controller class, so you don't have to repeat it for each
controller.

#### Others

You can also use the pre-injected
[AbpSession](/Pages/Documents/Abp-Session),
[EventBus](/Pages/Documents/EventBus-Domain-Events), [PermissionManager,
PermissionChecker](/Pages/Documents/Authorization),
[SettingManager](/Pages/Documents/Setting-Management), [FeatureManager,
FeatureChecker](/Pages/Documents/Feature-Management),
[LocalizationManager](/Pages/Documents/Localization),
[Logger](/Pages/Documents/Logging), and
[CurrentUnitOfWork](/Pages/Documents/Unit-Of-Work) base properties and
more.

### Filters

#### Exception Handling & Result Wrapping

All exceptions are automatically handled, logged, and a proper response
is returned to the client. See the [exception
handling](/Pages/Documents/Handling-Exceptions) documentation for more.

ASP.NET Boilerplate also **wraps** action results by default if the return
type is **JsonResult** (or Task&lt;JsonResult&gt; for async actions).

You can change exception handling and wrapping by using the **WrapResult**
and **DontWrapResult** attributes for controllers or actions or from the
[startup configuration](Startup-Configuration.md) (using
Configuration.Modules.AbpMvc()...) globally. See the [ajax
documentation](/Pages/Documents/Javascript-API/AJAX) for more info.

#### Audit Logging

The **AbpMvcAuditFilter** is used to integrate to the [audit logging
system](Audit-Logging.md). It logs all requests to all actions by
default (if auditing is not disabled). You can control audit logging
using the **Audited** and **DisableAuditing** attributes for actions and
controllers.

#### Validation

The **AbpMvcValidationFilter** automatically checks **ModelState.IsValid**
and prevents execution of the action if it's not valid. It also implements
input DTO validation described in the [validation
documentation](Validating-Data-Transfer-Objects.md).

#### Authorization

You can use **AbpMvcAuthorize** attribute for your controllers or
actions to prevent unauthorized users from using your controllers and
actions. Example:

    public class HomeController : AbpController
    {
        [AbpMvcAuthorize("MyPermissionName")]
        public ActionResult Index()
        {
            return View();
        }
    }

You can define the **AllowAnonymous** attribute for actions or controllers
to suppress authentication/authorization. The AbpController also defines an
**IsGranted** method as a shortcut to check permissions.

See the [authorization](/Pages/Documents/Authorization) documentation for
more info.

#### Unit Of Work

The **AbpMvcUowFilter** is used to integrate to the [Unit of
Work](Unit-Of-Work.md) system. It automatically begins a new unit of
work before an action execution, and if no exception is thrown, completes the unit of work 
after the action's execution.

You can use the **UnitOfWork** attribute to control the behaviour of the UOW for an
action. You can also use the startup configuration to change the default unit of
work attribute for all actions.

#### Anti Forgery

The **AbpAntiForgeryMvcFilter** is used to auto-protect MVC actions for
POST, PUT and DELETE requests from CSRF/XSRF attacks. See the [CSRF
documentation](XSRF-CSRF-Protection.md) for more.

### Model Binders

The **AbpMvcDateTimeBinder** is used to normalize DateTime (and
Nullable&lt;DateTime&gt;) inputs using the **Clock.Normalize** method.
