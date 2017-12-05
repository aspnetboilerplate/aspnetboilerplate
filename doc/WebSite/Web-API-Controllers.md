### Introduction

ASP.NET Boilerplate is integrated to **ASP.NET Web API Controllers** via
**Abp.Web.Api** nuget package. You can create regular ASP.NET Web API
Controllers as you always do. [Dependency
Injection](/Pages/Documents/Dependency-Injection) properly works for
regular ApiControllers. But you should derive your controllers from
**AbpApiController**, which provides several benefits and better
integrates to ASP.NET Boilerplate.

### AbpApiController Base Class

This is a simple api controller derived from **AbpApiController**:

    public class UsersController : AbpApiController
    {

    }

#### Localization

AbpApiController defines **L** method to make
[localization](/Pages/Documents/Localization) easier. Example:

    public class UsersController : AbpApiController
    {
        public UsersController()
        {
            LocalizationSourceName = "MySourceName";
        }

        public UserDto Get(long id)
        {
            var helloWorldText = L("HelloWorld");

            //...
        }
    }

You should set **LocalizationSourceName** to make **L** method working.
You can set it in your own base api controller class, to not repeat for
each api controller.

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

ABP defines some **pre-built filters** for AspNet Web API. All of them
are added to **all actions of all controllers** by default.

#### Audit Logging

**AbpApiAuditFilter** is used to integrate to [audit logging
system](Audit-Logging.html). It logs all requests to all actions by
default (if auditing is not disabled). You can control audit logging
using **Audited** and **DisableAuditing** attributes for actions and
controllers.

#### Authorization

You can use **AbpApiAuthorize** attribute for your api controllers or
actions to prevent unauthorized users to use your controllers and
actions. Example:

    public class UsersController : AbpApiController
    {
        [AbpApiAuthorize("MyPermissionName")]
        public UserDto Get(long id)
        {
            //...
        }
    }

You can define **AllowAnonymous** attribute for actions or controllers
to suppress authentication/authorization. AbpApiController also defines
**IsGranted** method as a shortcut to check permissions in the code.

See [authorization](/Pages/Documents/Authorization) documentation for
more. 

#### Anti Forgery Filter

**AbpAntiForgeryApiFilter** is used to auto protect ASP.NET Web API
actions (including [dynamic web api](Dynamic-Web-API.html)) for POST,
PUT and DELETE requests from CSRF/XSRF attacks. See [CSRF
documentation](XSRF-CSRF-Protection.html) for more. 

#### Unit Of Work

**AbpApiUowFilter** is used to integrate to [Unit of
Work](Unit-Of-Work.html) system. It automatically begins a new unit of
work before an action execution and completes unit of work after action
exucition (if no exception is thrown).

You can use **UnitOfWork** attribute to control behaviour of UOW for an
action. You can also use startup configuration to change default unit of
work attribute for all actions.

#### Result Wrapping & Exception Handling

ASP.NET Boilerplate **does not wrap** Web API actions **by default** if
action has successfully executed. But it **handles and wraps for
exceptions**. You can add WrapResult/DontWrapResult to actions and
controllers if you need. You can change this default behaviour from
[startup configuration](Startup-Configuration.html) (using
Configuration.Modules.AbpWebApi()...). See [AJAX
document](Javascript-API/AJAX.html) for more about result wrapping. 

#### Result Caching

ASP.NET Boilerplate adds Cache-Control header (no-cache, no-store) to
the response for Web API requests. Thus, it prevents browser caching of
responses even for GET requests. This behaviour can be disabled by the
configuration.

#### Validation

**AbpApiValidationFilter** automatically checks **ModelState.IsValid**
and prevents execution of the action if it's not valid. Also, implements
input DTO validation described in the [validation
documentation](Validating-Data-Transfer-Objects.html).

### Model Binders

**AbpApiDateTimeBinder** is used to normalize DateTime (and
Nullable&lt;DateTime&gt;) inputs using **Clock.Normalize** method.
